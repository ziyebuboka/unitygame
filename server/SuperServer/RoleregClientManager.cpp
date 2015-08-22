#include "zTCPClientTask.h"
#include "zTCPClientTaskPool.h"
#include "RoleregClientManager.h"
#include "RoleregClient.h"
#include "zXMLParser.h"
/*��ͳһ�û�ƽ̨�ͻ������ӵĹ�������
 */




/**
 * \brief ���Ψһʵ��ָ��
 */
RoleregClientManager *RoleregClientManager::instance = NULL;

/**
 * \brief ���캯��
 */
RoleregClientManager::RoleregClientManager()
{
  roleregClientPool = NULL;
}

/**
 * \brief ��������
 */
RoleregClientManager::~RoleregClientManager()
{
  SAFE_DELETE(roleregClientPool);
}

/**
 * \brief ��ʼ��������
 * \return ��ʼ���Ƿ�ɹ�
 */
bool RoleregClientManager::init()
{
  Zebra::logger->debug("RoleregClientManager::init");
  roleregClientPool = new zTCPClientTaskPool();
  if (NULL == roleregClientPool
      || !roleregClientPool->init())
    return false;

  zXMLParser xml;
  if (!xml.initFile(Zebra::global["configdir"] + "loginServerList.xml"))
  {
    Zebra::logger->error("����ͳһ�û�ƽ̨��½�������б��ļ�ʧ��");
    return false;
  }
  xmlNodePtr root = xml.getRootNode("Zebra");
  if (root)
  {
    xmlNodePtr zebra_node = xml.getChildNode(root,"RoleregServerList");
    while(zebra_node)
    {
      if (strcmp((char *)zebra_node->name,"RoleregServerList") == 0)
      {
        xmlNodePtr node = xml.getChildNode(zebra_node,"server");
        while(node)
        {
          if (strcmp((char *)node->name,"server") == 0)
          {
	     Zebra::global["RoleregServer"]   = "";
            Zebra::global["RoleregPort"]   = "";

            if (xml.getNodePropStr(node,"ip", Zebra::global["RoleregServer"])
                && xml.getNodePropStr(node,"port", Zebra::global["RoleregPort"]))
            {
              Zebra::logger->debug("LoginServer: %s,%s",Zebra::global["RoleregServer"].c_str(),Zebra::global["RoleregPort"].c_str());
              roleregClientPool->put(new RoleregClient(Zebra::global["RoleregServer"], atoi(Zebra::global["RoleregPort"].c_str())));
            }
          }

          node = xml.getNextNode(node,NULL);
        }
      }

      zebra_node = xml.getNextNode(zebra_node,NULL);
    }
  }
  Zebra::logger->info("RoleregClientManager::init OK");
  return true;
}

/**
 * \brief ���ڼ���������ӵĶ�����������
 * \param ct ��ǰʱ��
 */
void RoleregClientManager::timeAction(const zTime &ct)
{
  Zebra::logger->debug("RoleregClientManager::timeAction");
  if (actionTimer.elapse(ct) > 4)
  {
    if (roleregClientPool)
      roleregClientPool->timeAction(ct);
    actionTimer = ct;
  }
}

/**
 * \brief �������������Ѿ��ɹ�������
 * \param RoleregClient �����ӵ�����
 */
void RoleregClientManager::add(RoleregClient *roleregClient)
{
  Zebra::logger->debug("RoleregClientManager::add");
  if (roleregClient)
  {
    zRWLock_scope_wrlock scope_wrlock(rwlock);
    const_iter it = find(allClients.begin(), allClients.end(), roleregClient);
    if (it == allClients.end())
    {
      allClients.insert(roleregClient);
    }
  }
}

/**
 * \brief ���������Ƴ��Ͽ�������
 * \param RoleregClient ���Ƴ�������
 */
void RoleregClientManager::remove(RoleregClient *roleregClient)
{
  Zebra::logger->debug("RoleregClientManager::remove");
  if (roleregClient)
  {
    zRWLock_scope_wrlock scope_wrlock(rwlock);
    iter it = find(allClients.begin(), allClients.end(), roleregClient);
    if (it != allClients.end())
    {
      allClients.erase(it);
    }
  }
}

/**
 * \brief ��ɹ����������ӹ㲥ָ��
 * \param pstrCmd ���㲥��ָ��
 * \param nCmdLen ���㲥ָ��ĳ���
 */
bool RoleregClientManager::broadcastOne(const void *pstrCmd,int nCmdLen)
{
  Zebra::logger->debug("RoleregClientManager::broadcastOne");
  zRWLock_scope_rdlock scope_rdlock(rwlock);
  for(iter it = allClients.begin(); it != allClients.end(); ++it)
  {
      if((*it)->sendCmd(pstrCmd,nCmdLen))
	  return true;
  }
  return false;
}


MLoader("MyLua.Libs.Core.GlobalNS");
MLoader("MyLua.Libs.Core.Class");
MLoader("MyLua.Libs.UI.UICore.Form");

MLoader("MyLua.Libs.AuxComponent.AuxUIComponent.AuxButton");

MLoader("MyLua.UI.UIOptionPanel.OptionPanelNS");
MLoader("MyLua.UI.UIOptionPanel.OptionPanelData");
MLoader("MyLua.UI.UIOptionPanel.OptionPanelCV");

--UI区
local M = GlobalNS.Class(GlobalNS.Form);
M.clsName = "UIOptionPanel";
GlobalNS.OptionPanelNS[M.clsName] = M;

function M:ctor()
	self.mId = GlobalNS.UIFormID.eUIOptionPanel;
	self.mData = GlobalNS.new(GlobalNS.OptionPanelNS.OptionPanelData);
end

function M:dtor()
	
end

function M:onInit()
    M.super.onInit(self);
	
	self.mSplitBtn = GlobalNS.new(GlobalNS.AuxButton);
	self.mSplitBtn:addEventHandle(self, self.onSplitBtnClk);
end

function M:onReady()
    M.super.onReady(self);
	self.mSplitBtn:setSelfGo(GlobalNS.UtilApi.TransFindChildByPObjAndPath(
			self.mGuiWin, 
			GlobalNS.OptionPanelNS.OptionPanelPath.BtnSplit)
		);
end

function M:onShow()
    M.super.onShow(self);
end

function M:onHide()
    M.super.onHide(self);
end

function M:onExit()
    M.super.onExit(self);
end

function M:onSplitBtnClk()
	GCtx.mLogSys:log("Split", GlobalNS.LogTypeId.eLogCommon);
end

return M;
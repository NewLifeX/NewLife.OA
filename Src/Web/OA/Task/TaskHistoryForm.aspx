﻿<%@ Page Title="任务历史管理" Language="C#" MasterPageFile="~/Admin/ManagerPage.master" AutoEventWireup="true" CodeFile="TaskHistoryForm.aspx.cs" Inherits="OA_TaskHistoryForm"%>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="C">
    <table border="0" class="m_table" cellspacing="1" cellpadding="0" align="Center">
        <tr>
            <th colspan="2">任务历史</th>
        </tr>
        <tr>
            <td align="right">任务：</td>
            <td><XCL:NumberBox ID="frmWorkTaskID" runat="server" Width="80px"></XCL:NumberBox></td>
        </tr>
<tr>
            <td align="right">种类：</td>
            <td><XCL:NumberBox ID="frmKind" runat="server" Width="80px"></XCL:NumberBox></td>
        </tr>
<tr>
            <td align="right">原来的值：</td>
            <td><asp:TextBox ID="frmSrcValue" runat="server" Width="150px"></asp:TextBox></td>
        </tr>
<tr>
            <td align="right">新的值：</td>
            <td><asp:TextBox ID="frmNewValue" runat="server" Width="150px"></asp:TextBox></td>
        </tr>
<tr>
            <td align="right">创建者：</td>
            <td><XCL:NumberBox ID="frmCreateUserID" runat="server" Width="80px"></XCL:NumberBox></td>
        </tr>
<tr>
            <td align="right">创建时间：</td>
            <td><XCL:DateTimePicker ID="frmCreateTime" runat="server"></XCL:DateTimePicker></td>
        </tr>
<tr>
            <td align="right">更新者：</td>
            <td><XCL:NumberBox ID="frmUpdateUserID" runat="server" Width="80px"></XCL:NumberBox></td>
        </tr>
<tr>
            <td align="right">更新时间：</td>
            <td><XCL:DateTimePicker ID="frmUpdateTime" runat="server"></XCL:DateTimePicker></td>
        </tr>
<tr>
            <td align="right">备注：</td>
            <td><asp:TextBox ID="frmRemark" runat="server" TextMode="MultiLine" Width="300px" Height="80px"></asp:TextBox></td>
        </tr>
    </table>
    <table border="0" align="Center" width="100%">
        <tr>
            <td align="center">
                <asp:Button ID="btnSave" runat="server" CausesValidation="True" Text='保存' />
                &nbsp;<asp:Button ID="btnCopy" runat="server" CausesValidation="True" Text='另存为新任务历史' />
                &nbsp;<asp:Button ID="btnReturn" runat="server" OnClientClick="parent.Dialog.CloseSelfDialog(frameElement);return false;" Text="返回" />
            </td>
        </tr>
    </table>
</asp:Content>
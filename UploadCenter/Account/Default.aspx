<%@ Page Title="My Uploaded Files" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="UploadCenter.Account.Default" %>

<%@ Import Namespace="UploadCenter.Classes" %>
<%@ Register Src="~/FilesDetail.ascx" TagPrefix="uc1" TagName="FilesDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plhHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plhBody" runat="server">
	<div class="panel panel-primary">
		<div class="panel-heading">
			Your uploaded files
		</div>
		<div class="panel-body form-horizontal" role="form">
			<div id="boxMessage" class="alert alert-info fade in" runat="server" visible="False" enableviewstate="False">
				You have no uploaded files.
			</div>
			<div class="alert alert-info" runat="server" id="boxCount">
				<strong>Summary:</strong>
				<asp:Label Text="0" runat="server" ID="lblFilesCount" EnableViewState="False"></asp:Label>
			</div>
			<uc1:FilesDetail RedirectAfterDelete="true" RedirectAfterDeleteLocation="/account/" runat="server" ID="ucFiles" />
		</div>
	</div>

</asp:Content>

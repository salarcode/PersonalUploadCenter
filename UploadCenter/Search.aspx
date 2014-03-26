<%@ Page Title="Search in Files" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="UploadCenter.Search" %>
<%@ Register TagPrefix="uc1" TagName="FilesDetail" Src="~/FilesDetail.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plhHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plhBody" runat="server">
	<div class="panel panel-primary">
		<h1 class="panel-heading">
			Search Result
		</h1>
		<div class="panel-body form-horizontal" role="form">
			<div id="vldErrorsBox" class="alert alert-danger fade in" runat="server" visible="False" enableviewstate="False">
				<asp:Label ID="vldErrors" runat="server" EnableViewState="False"></asp:Label>
			</div>
			<uc1:FilesDetail runat="server" ID="ucFiles" />
		</div>
	</div>
</asp:Content>

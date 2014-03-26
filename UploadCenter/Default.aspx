<%@ Page Title="Upload Center - Upload your files from PC or remote URL" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="UploadCenter.Default" %>
<%@ Register TagPrefix="uc1" TagName="FilesDetail" Src="~/FilesDetail.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plhHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plhBody" runat="server">
	<div class="jumbotron">
		<h2>Welcome</h2>
		<p>Welcome to the Upload Center, a place where you are able to upload your files and share them with your friends. You can upload files either from your PC or a remote Url.</p>
		<h1><a href="/frompc.aspx" class="btn btn-primary" role="button">Upload from PC</a> <a href="/remote.aspx" class="btn btn-primary" role="button">Upload from Remote</a></h1>
	</div>
	<div id="boxLatestFiles" runat="server" class="panel panel-primary">
		<h1 class="panel-heading">
			Latest Public Uploaded Files
		</h1>
		<div class="panel-body form-horizontal" role="form">
			<uc1:FilesDetail runat="server" ID="ucFiles" />
		</div>
	</div>

</asp:Content>

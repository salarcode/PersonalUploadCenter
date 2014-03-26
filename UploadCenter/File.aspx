<%@ Page Title="View Uploaded File" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="File.aspx.cs" Inherits="UploadCenter.File" %>
<%@ Register src="FilesDetail.ascx" tagname="FilesDetail" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plhHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plhBody" runat="server">
	<script>
		$('#view-file').addClass('active');
	</script>
	<style>
		.cookie-bar-default {
		}
	</style>
	<div class="panel panel-primary">
		<h1 class="panel-heading">
			View Uploaded File
		</h1>
		<div class="panel-body" role="form">
			<uc1:FilesDetail RedirectAfterDelete="True" ID="ucFiles" runat="server" />
		</div>
	</div>
</asp:Content>

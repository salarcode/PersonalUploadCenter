<%@ Page Title="Upload From PC" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FromPc.aspx.cs" Inherits="UploadCenter.FromPc" %>
<%@ Import Namespace="UploadCenter.Classes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plhHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plhBody" runat="server">
	<script>
		$('#menu-from-pc').addClass('active');
	</script>
	<div class="panel panel-primary">
		<h1 class="panel-heading">
			Upload from PC
		</h1>
		<div class="panel-body form-horizontal" role="form">
			<div id="vldErrorsBox" class="alert alert-danger fade in" runat="server" visible="False" enableviewstate="False">
				<button class="close" aria-hidden="true" data-dismiss="alert" type="button">×</button>
				<asp:Label ID="vldErrors" runat="server"  EnableViewState="False"></asp:Label>
			</div>
			<div class="form-group">
				<label class="col-sm-2" for="<%:filePcUpload.ClientID %>">Select File:</label>
				<div class="col-sm-10">
					<asp:FileUpload accept="*/*" CssClass="form-control" ID="filePcUpload" runat="server"  ValidationGroup="FromPc"/>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-2" for="<%:txtNewName.ClientID %>">Rename:</label>
				<div class="col-sm-10">
					<asp:TextBox ID="txtNewName" onchange="CheckExtention(this.value)" Columns="40" CssClass="form-control"
						runat="server" ValidationGroup="FromPc"></asp:TextBox>
					<span class="text-muted label label-default">optional</span>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-2" for="<%:txtRemoteComment.ClientID %>">Comment:</label>
				<div class="col-sm-10">
					<asp:TextBox ID="txtRemoteComment" Columns="40" runat="server" ValidationGroup="FromPc" CssClass=" col-sm-10 form-control"></asp:TextBox>
					<span class="text-muted label label-default">optional</span>
				</div>
			</div>
			
			<asp:HiddenField ID="txtVisibility" runat="server" Value="1" />
			<% if (UserManager.SignedIn()){ %>
			<div class="form-group">
				<label class="col-sm-2">Visible to:</label>
				<div class="col-sm-10">
					<div id="visibility_parent" class="btn-group" data-toggle="buttons">
						<label class="btn btn-default " onclick="setVisiblity(1)">
							<input type="radio" name="Visiblity" id="visibility1" value="1" onclick="setVisiblity(1)" />
							<i class="glyphicon glyphicon-globe"></i>
							Public
						</label>
						<label class="btn btn-default" onclick="setVisiblity(0)">
							<input type="radio" name="Visiblity" id="visibility2" value="0" onclick="setVisiblity(0)" />
							<i class="glyphicon glyphicon-eye-close"></i>
							Me Only
						</label>
					</div>
				</div>
			</div>
			<script>
				function setVisiblity(value) {
					var elem = $('#<%:txtVisibility.ClientID%>');
					elem.val(value);
				}

				function readVisiblity() {
					var elem = $('#<%:txtVisibility.ClientID%>').val();
					if (elem == "0") {
						$('#visibility_parent label:nth-child(2)').addClass("active");
					} else {
						$('#visibility_parent label:nth-child(1)').addClass("active");
					}
				}
				readVisiblity();
			</script>
			<% } %>

			<div class="form-group">
				<label class="col-sm-2"></label>
				<div class="col-sm-10">
					<asp:HiddenField ID="txtCookies" runat="server" />
					<button id="btnFromPcUpload" runat="server" ValidationGroup="FromPc" class="btn btn-primary " onserverclick="btnFromPcUpload_Click">
						<i class="glyphicon glyphicon-open"></i>
						Upload
					</button>
				</div>
			</div>
		</div>
	</div>
</asp:Content>

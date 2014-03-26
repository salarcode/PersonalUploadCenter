<%@ Page Title="Login to System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UploadCenter.Account.Login" %>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="plhBody">
	<script>
		$('#menu-login').addClass('active');
	</script>
	<div class="panel panel-primary">
		<div class="panel-heading">
			Login to System
		</div>
		<div class="panel-body form-horizontal" role="form">
			<div id="vldErrorsBox" class="alert alert-danger fade in" runat="server" visible="False" enableviewstate="False">
				<button class="close" aria-hidden="true" data-dismiss="alert" type="button">×</button>
				<asp:Label ID="vldErrors" runat="server" EnableViewState="False"></asp:Label>
			</div>
			<div class="form-group">
				<label class="col-sm-2" for="<%:txtUsername.ClientID %>">Username:</label>
				<div class="col-sm-5">
					<asp:TextBox ID="txtUsername" Columns="40" CssClass="form-control"
						runat="server" ValidationGroup="Remote"></asp:TextBox>
					<asp:RequiredFieldValidator ValidationGroup="Remote" ID="vldUsername" runat="server" ControlToValidate="txtUsername" Display="Dynamic" ErrorMessage="Username is required." ForeColor="Red"></asp:RequiredFieldValidator>
				</div>
			</div>

			<div class="form-group">
				<label class="col-sm-2" for="<%:txtPassword.ClientID %>">Password:</label>
				<div class="col-sm-5">
					<asp:TextBox TextMode="Password" ID="txtPassword" Columns="40" CssClass="form-control"
						runat="server" ValidationGroup="Remote"></asp:TextBox>
					<asp:RequiredFieldValidator ValidationGroup="Remote" ID="vldPassword" runat="server" ControlToValidate="txtPassword" Display="Dynamic" ErrorMessage="Password is required." ForeColor="Red"></asp:RequiredFieldValidator>
				</div>
			</div>

			<div class="form-group">
				<label class="col-sm-2"></label>
				<div class="col-sm-5">
					<asp:Button ID="btnLogin" ValidationGroup="Remote" CssClass="btn btn-primary " runat="server" Text="Login" OnClick="btnLogin_Click" />
				</div>
			</div>
		</div>
	</div>
</asp:Content>

<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="UploadCenter.Account.Register" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="plhBody">
	<script>
		$('#menu-register').addClass('active');
	</script>
	<div class="panel panel-primary">
		<div class="panel-heading">
			<asp:Label ID="lblRegisterHead" Visible="False" runat="server" Text="Register an Account" Font-Bold="True"></asp:Label>
			<asp:Label ID="lblRegisterAdminHead" Visible="False" runat="server" Text="Register Administrator" Font-Bold="True"></asp:Label>
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
				<label class="col-sm-2" for="<%:txtPasswordConfirm.ClientID %>">Password Confirm:</label>
				<div class="col-sm-5">
					<asp:TextBox TextMode="Password" ID="txtPasswordConfirm" Columns="40" CssClass="form-control"
						runat="server" ValidationGroup="Remote"></asp:TextBox>
					<asp:CompareValidator ValidationGroup="Remote" ID="vldPasswordConfirm" runat="server" ControlToCompare="txtPassword" ControlToValidate="txtPasswordConfirm" Display="Dynamic" ErrorMessage="Password confirm is not correct." ForeColor="Red"></asp:CompareValidator>
				</div>
			</div>

			<div class="form-group">
				<label class="col-sm-2" for="<%:txtEmail.ClientID %>">Email:</label>
				<div class="col-sm-5">
					<asp:TextBox ID="txtEmail" Columns="40" CssClass="form-control"
						runat="server" ValidationGroup="Remote"></asp:TextBox>
					<asp:RegularExpressionValidator ValidationGroup="Remote" ID="vldEmail" runat="server" ErrorMessage="Please enter a valid email address." ControlToValidate="txtEmail" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ForeColor="Red"></asp:RegularExpressionValidator>
					<asp:RequiredFieldValidator ValidationGroup="Remote" ID="vldEmailReq" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Email is required." ForeColor="Red"></asp:RequiredFieldValidator>
				</div>
			</div>

			<div class="form-group">
				<label class="col-sm-2" for="<%:txtEmailConfirm.ClientID %>">Email Confirm:</label>
				<div class="col-sm-5">
					<asp:TextBox ID="txtEmailConfirm" Columns="40" CssClass="form-control"
						runat="server" ValidationGroup="Remote"></asp:TextBox>
					<asp:CompareValidator ValidationGroup="Remote" ID="vldEmailConfirm" runat="server" ControlToCompare="txtEmail" ControlToValidate="txtEmailConfirm" Display="Dynamic" ErrorMessage="Email confirm is not correct." ForeColor="Red"></asp:CompareValidator>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-2"></label>
				<div class="col-sm-5">
					<asp:Button ID="btnRegister" ValidationGroup="Remote" CssClass="btn btn-primary " runat="server" Text="Register" OnClick="btnRegister_Click" />
				</div>
			</div>

		</div>
	</div>
</asp:Content>

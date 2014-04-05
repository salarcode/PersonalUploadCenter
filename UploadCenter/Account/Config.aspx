<%@ Page Title="Upload Center Configuration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Config.aspx.cs" Inherits="UploadCenter.Account.Config" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plhHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plhBody" runat="server">
	<script>
		$('#menu-config').addClass('active');
	</script>
	<div class="panel panel-primary">
		<div class="panel-heading">
			System Configurations
		</div>
		<div class="panel-body form-horizontal" role="form">
			<div class="form-group">
				<label class="col-sm-2" for="<%:AllowRegister.ClientID %>">AllowRegister:</label>
				<div class="col-sm-5">
					<asp:CheckBox ID="AllowRegister" Text="AllowRegister" Columns="40" CssClass=""
						runat="server" ValidationGroup="Remote"></asp:CheckBox>
				</div>
			</div>

			<div class="form-group">
				<label class="col-sm-2" for="<%:AllowRemoteUpload.ClientID %>">AllowRemoteUpload:</label>
				<div class="col-sm-5">
					<asp:CheckBox ID="AllowRemoteUpload" Text="AllowRemoteUpload" Columns="40" CssClass=""
						runat="server" ValidationGroup="Remote"></asp:CheckBox>
				</div>
			</div>

			<div class="form-group">
				<label class="col-sm-2" for="<%:AllowPcUpload.ClientID %>">AllowPcUpload:</label>
				<div class="col-sm-5">
					<asp:CheckBox ID="AllowPcUpload" Text="AllowPcUpload" Columns="40" CssClass=""
						runat="server" ValidationGroup="Remote"></asp:CheckBox>
				</div>
			</div>

			<div class="form-group">
				<label class="col-sm-2" for="<%:MaxFileSizeBytes.ClientID %>">MaxFileSizeBytes:</label>
				<div class="col-sm-5">
					<asp:TextBox type="number" ID="MaxFileSizeBytes" Columns="40" CssClass="form-control"
						runat="server" ValidationGroup="Remote"></asp:TextBox>
					<asp:RequiredFieldValidator ValidationGroup="Remote" ID="RequiredFieldValidator3" runat="server" 
						ControlToValidate="MaxFileSizeBytes" Display="Dynamic" ErrorMessage="Username is required." ForeColor="Red"></asp:RequiredFieldValidator>
				</div>
			</div>

		</div>
	</div>
</asp:Content>

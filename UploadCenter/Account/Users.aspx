<%@ Page Title="Users List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="UploadCenter.Account.Users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plhHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plhBody" runat="server">
	<script>
		$('#menu-register').addClass('active');
	</script>
	<div class="panel panel-primary">
		<div class="panel-heading">
			System Users
		</div>
		<div class="panel-body form-horizontal" role="form">
			<asp:GridView Width="90%" CssClass="table table-responsive" ID="grdUsers" runat="server" EnableViewState="False" CellPadding="4" ForeColor="#333333" GridLines="None">
				<AlternatingRowStyle BackColor="White" />
				<EditRowStyle BackColor="#2461BF" />
				<FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
				<HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
				<PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
				<RowStyle BackColor="#EFF3FB" />
				<SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
				<SortedAscendingCellStyle BackColor="#F5F7FB" />
				<SortedAscendingHeaderStyle BackColor="#6D95E1" />
				<SortedDescendingCellStyle BackColor="#E9EBEF" />
				<SortedDescendingHeaderStyle BackColor="#4870BE" />
			</asp:GridView>
		</div>
	</div>
</asp:Content>

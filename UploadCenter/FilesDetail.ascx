<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FilesDetail.ascx.cs" Inherits="UploadCenter.FilesDetail" %>
<%@ Import Namespace="UploadCenter.Database" %>
<div id="vldErrorsBox" class="alert alert-danger fade in" runat="server" visible="False" enableviewstate="False">
	<button class="close" aria-hidden="true" data-dismiss="alert" type="button">×</button>
	<asp:Label ID="vldErrors" runat="server" EnableViewState="False"></asp:Label>
</div>
<script>
	filesDetail = {
		writeDateTime: function (dt) {
			if (dt == '') return;
			document.write(moment(dt).format('YYYY/MM/DD HH:mm:ss'));
		},
		updateDateTime: function (dt, elm) {
			if (dt == '') return;
			$(elm).text(moment(dt).format('YYYY/MM/DD HH:mm:ss'));
		}
	};
</script>
<asp:Repeater ID="rptRepeat" runat="server" OnItemCommand="rptRepeat_ItemCommand">
	<HeaderTemplate></HeaderTemplate>
	<FooterTemplate></FooterTemplate>
	<SeparatorTemplate>
		<hr />
	</SeparatorTemplate>
	<ItemTemplate>
		<div class="row">
			<div class="col-xs-3 col-sm-2 col-md-1 text-center">
				<div>
					<span class="icon file ext-<%#((UploadedFile)Container.DataItem).DisplayFileExtension%>"></span>
				</div>
				<div runat="server" visible='<%# ((UploadedFile)Container.DataItem).VisitorIsOwner %>'>
					<asp:Button CommandName="Delete" CommandArgument="<%# ((UploadedFile)Container.DataItem).UploadedFileID %>" runat="server" UseSubmitBehavior="False" CssClass="btn btn-default btn-sm" ID="btnDelete" OnClientClick="if(!showConfirm())return;" title="Delete" Text="x" />
					<script>
						function showConfirm() {
							return confirm("Are you sure to delete this file?");
						}
					</script>
				</div>
			</div>
			<div class="col-xs-9 col-sm-10">
				<div class="alert alert-success">
					<strong><a rel="nofollow" href="/download.ashx?id=<%#((UploadedFile)Container.DataItem).UploadedFileID %>"><i class="glyphicon glyphicon-circle-arrow-down"></i><%#((UploadedFile)Container.DataItem).Filename %></a></strong>
				</div>
				<div>
					<span class="col-sm-2">Size:</span>
					<span class="label label-default"><%#((UploadedFile)Container.DataItem).FileSizeName %></span>
				</div>
				<div>
					<span class="col-sm-2">Downloaded:</span>
					<span class="label label-default"><%#((UploadedFile)Container.DataItem).Downloaded %></span>
				</div>
				<div>
					<span class="col-sm-2">Uploaded:</span>
					<span class="label label-default">
						<script type="text/javascript">
							filesDetail.writeDateTime('<%#((UploadedFile)Container.DataItem).UploadDate.ToString("R") %>');
						</script>
					</span>
				</div>
				<div>
					<span class="col-sm-2">Last Download:</span>
					<span class="label label-default" id="LastDownload">
						<script type="text/javascript">filesDetail.writeDateTime('<%#(((UploadedFile)Container.DataItem).LastDownload==null?"":((UploadedFile)Container.DataItem).LastDownload.Value.ToString("R")) %>');</script>
					</span>
				</div>
				<div>
					<span class="col-sm-2">Uploader:</span>
					<span class="label label-default"><%#((UploadedFile)Container.DataItem).UploaderUsername %></span>&nbsp;
				</div>
				<div runat="server" visible='<%# ((UploadedFile)Container.DataItem).VisitorIsOwner %>'>
					<span class="col-sm-2">Access:</span>
					<span class="label label-default"><%#((UploadedFile)Container.DataItem).IsPublic?"Public":"Private" %></span>&nbsp;
				</div>
				<div>
					<span class="col-sm-2">Comment:</span>
					<span class=""><%#((UploadedFile)Container.DataItem).Comment %></span>&nbsp;
				</div>
			</div>
		</div>

	</ItemTemplate>
</asp:Repeater>

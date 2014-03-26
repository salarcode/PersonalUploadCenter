<%@ Page Title="Upload from Remote Url" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Remote.aspx.cs" Inherits="UploadCenter.Remote" %>

<%@ Import Namespace="UploadCenter.Classes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plhHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plhBody" runat="server">
	<script>
		$('#menu-from-remote').addClass('active');
	</script>
	<style>
		.cookie-bar-default {
		}
	</style>
	<div class="panel panel-primary">
		<h1 class="panel-heading">
			Upload from remote url
		</h1>
		<div class="panel-body form-horizontal" role="form">
			<div id="vldErrorsBox" class="alert alert-danger fade in" runat="server" visible="False" enableviewstate="False">
				<button class="close" aria-hidden="true" data-dismiss="alert" type="button">×</button>
				<asp:Label ID="vldErrors" runat="server" EnableViewState="False"></asp:Label>
			</div>
			<div class="form-group">
				<label class="col-sm-2" for="<%:txtRemoteUrl.ClientID %>">File Url:</label>
				<div class="col-sm-10">
					<asp:TextBox ID="txtRemoteUrl" Columns="60" runat="server" ValidationGroup="Remote" CssClass="form-control"></asp:TextBox>
					<asp:RegularExpressionValidator ID="vldUrl" runat="server" ControlToValidate="txtRemoteUrl"
						ErrorMessage="Enter a valid URL" ValidationExpression="(\w+):\/\/([\w.]+\/?)\S*"
						ValidationGroup="Remote" Display="Dynamic"></asp:RegularExpressionValidator>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-2" for="<%:txtNewName.ClientID %>">Rename:</label>
				<div class="col-sm-10">
					<asp:TextBox ID="txtNewName" onchange="CheckExtention(this.value)" Columns="40" CssClass="form-control"
						runat="server" ValidationGroup="Remote"></asp:TextBox>
					<span class="text-muted label label-default">optional</span>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-2" for="<%:txtRemoteComment.ClientID %>">Comment:</label>
				<div class="col-sm-10">
					<asp:TextBox ID="txtRemoteComment" Columns="40" runat="server" ValidationGroup="Remote" CssClass=" col-sm-10 form-control"></asp:TextBox>
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
				<div class="col-sm-10 panel-group" id="accordion">
					<div class="panel panel-primary">
						<div class="panel-heading">
							<h4 class="panel-title">
								<a data-toggle="collapse" data-parent="#accordion" href="#advancedOptions">Advanced Options
								</a>
							</h4>
						</div>
						<div id="advancedOptions" class="panel-collapse collapse in-no">
							<div class="panel-body">
								<fieldset>
									<legend>Cookies
										<button type="button" class="btn btn-default" onclick="page.addCookieBar()">Add New</button>
									</legend>
									<div id="cookies-holder">
										<div class="cookie-bar-default form-group row">
											<div class="form-horizontal">
												<div class="col-sm-1">
													<button type="button" class="btn btn-default" onclick="page.deleteCookie(this)">Delete</button>
												</div>
												<div class="col-sm-3">
													<label class="col-sm-3 control-label">Name:</label>
													<div class="col-sm-9">
														<input id="cookie-name" type="text" class="form-control" />
													</div>
												</div>
												<div class="col-sm-5">
													<label class="col-sm-3 control-label">Content:</label>
													<div class="col-sm-9">
														<input id="cookie-content" type="text" class="form-control" />
													</div>
												</div>
												<div class="col-sm-3">
													<label class="col-sm-3 control-label">Path:</label>
													<div class="col-sm-9">
														<input id="cookie-path" type="text" class="form-control" value="/" />
													</div>
												</div>
											</div>
										</div>
									</div>
								</fieldset>
							</div>
						</div>
					</div>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-2"></label>
				<div class="col-sm-10">
					<asp:HiddenField ID="txtCookies" runat="server" />
					<button id="btnRemoteUpload" ValidationGroup="Remote" runat="server" onclick="page.buildTheCookies();" class="btn btn-primary " onserverclick="btnRemoteUpload_Click">
						<i class="glyphicon glyphicon-open"></i>
						Upload
					</button>

				</div>
			</div>

		</div>
	</div>

	<script>
		jQuery.fn.outerHTML = function (s) {
			return s
				? this.before(s).remove()
				: jQuery("<p>").append(this.eq(0).clone()).html();
		};
		page = {
			cookiesBarTemplate: $('.cookie-bar-default').outerHTML(),
			addCookieBar: function () {
				$('#cookies-holder').append($(page.cookiesBarTemplate));
			},
			deleteCookie: function (e) {
				if (e == null) return;
				$(e).parents('.cookie-bar-default').remove();
			},
			buildTheCookies: function () {
				var array = new Array();
				$('.cookie-bar-default').each(function (index, item) {
					var name = $(item).find('#cookie-name').val();
					var content = $(item).find('#cookie-content').val();
					var path = $(item).find('#cookie-path').val();

					array.push({
						Name: name,
						Content: content,
						Path: path
					});
				});

				var txtCookies = $('#<%:txtCookies.ClientID%>');
				txtCookies.val(JSON.stringify(array));
			},
			readTheCookies: function () {
				var txtCookies = $('#<%:txtCookies.ClientID%>');
				var cookiesVal = txtCookies.val();

				$('#cookies-holder .cookie-bar-default').remove();
				if (cookiesVal == '')
					return;
				var cookies = JSON.parse(cookiesVal);
				for (var i = 0; i < cookies.length; i++) {
					var item = cookies[i];
					var bar = $(page.cookiesBarTemplate);

					bar.find('#cookie-name').val(item.Name);
					bar.find('#cookie-content').val(item.Content);
					bar.find('#cookie-path').val(item.Path);

					$('#cookies-holder').append(bar);
				}
			}
		};

		page.readTheCookies();
	</script>
</asp:Content>

var Quickafe = (function () {
    var loadingTemplate = "<div class='k-loading-image'></div>";
    var validationHelper = {
        errorTemplate: "<span class='label label-danger'>#=message#</span>",

        remoteValidationRule: function (input) {
            var remoteAttr = input.attr("data-val-remote-url");
            if (typeof remoteAttr === typeof undefined || remoteAttr === false) {
                return true;
            }

            var isInvalid = true;
            var data = {};

            data[input.attr('name')] = input.val();

            var additionalFieldsAttr = input.attr("data-val-remote-additionalfields");
            if (additionalFieldsAttr != undefined) {
                var additionalFields = additionalFieldsAttr.split(",");
                $.each(additionalFields, function (index, arrayEl) {
                    data[arrayEl.substring(2)] = $("#" + arrayEl.substring(2)).val();
                });
            }

            $.ajax({
                url: remoteAttr,
                mode: "abort",
                port: "validate" + input.attr('name'),
                dataType: "json",
                type: input.attr("data-val-remote-type"),
                data: data,
                async: false,
                success: function (response) {
                    isInvalid = response;
                }
            });
            return !isInvalid;
        },

        remoteValidationMessage: function (input) {
            return input.data('val-remote');
        }
    };

    var validatorOptions = {
        errorTemplate: validationHelper.errorTemplate,
        rules: {
            remote: validationHelper.remoteValidationRule
        },
        messages: {
            remote: validationHelper.remoteValidationMessage
        }
    };

    var reportWindowOptions = {
        actions: ["Maximize", "Close"],
        height: window.screen.availHeight - 200,
        width: window.screen.availWidth - 400,
        modal: true,
        iframe: true,
        animation: false,
        visible: false,
        title: "Print Preview"
    };

    function refreshGrid(gridId) {
        if (gridId === undefined) {
            $("#grid").data("kendoGrid").dataSource.read();
            $("#grid").data("kendoGrid").dataSource.page(1);
            $("#grid").data("kendoGrid").refresh();
        }
        else {
            $(gridId).data("kendoGrid").dataSource.read();
            $(gridId).data("kendoGrid").dataSource.page(1);
            $(gridId).data("kendoGrid").refresh();
        }
    }

    function onLogout(e) {
        e.preventDefault();
        var that = this;
        if (bootbox.confirm('Are you sure want to logout?', function (result) {           
            if (result) {
                document.location.href = $(that).attr("href");
            }
        }));
    }

    return {
        validatorOptions: validatorOptions,
        reportWindowOptions: reportWindowOptions,
        loadingTemplate: loadingTemplate,
        refreshGrid: refreshGrid,
        onLogout: onLogout
    };
})();

window.kendoPrint = (function () {
    var html = "<div />";
    var win = $(html).kendoWindow(Quickafe.reportWindowOptions).getKendoWindow();

    return function (reportPreviewUrl) {
        win.refresh(reportPreviewUrl);
        win.center().open();
    }
})();

// attach kendoAlert to the window
window.kendoAlert = (function () {

    var defaultIcon = "<i class='fa fa-coffee'></i>";
    var dfrd;
    var html = "<div id='kendoAlert' style='min-height:100px; width:400px; text-align: center; vertical-align:center;' />";
    // create modal window on the fly
    var win = $(html).kendoWindow({
        modal: true,
        animation: false,
        deactivate: function () {
            // when the deactivate event fires,
            // resolve the promise
            dfrd.resolve();
        }
    }).getKendoWindow();

    return function (msg, title) {
        if (title == undefined)
            title = defaultIcon + " Point of Sales";
        else
            title = defaultIcon + " " + title;
        msg = "&nbsp;&nbsp;&nbsp;" + msg + "&nbsp;&nbsp;&nbsp;";
        // create a new deferred
        dfrd = $.Deferred();

        // set the content
        win.content(msg);
        win.element.append($('<div style="vertical-align:bottom; padding-top:50px;"><a id="kendoAlertClose" class="k-button"><i class="fa fa-check"></i> OK</a></div>'));

        $("#kendoAlertClose", "#kendoAlert").on('click', function () {
            $(this).closest("[data-role=window]").data("kendoWindow").close();
        });

        // center it and open it
        win.center().title(title).open();

        // return the deferred object
        return dfrd;
    };

}());

$(function () {
    $("#logout").click(Quickafe.onLogout);
    $("#aboutMenu").click(function (e) {
        var windowOptions = {
            modal: true,
            width: 450,
            height: 200,
            title: "About",                
            content: $(this).data("abouturl")
        };
        $("#about").html(Quickafe.loadingTemplate);
        $("#about").kendoWindow(windowOptions).data("kendoWindow").open().center();
    });
});


// JQuery extension
(function ($) {
    $.fn.focusEnd = function () {
        this.focus();
        var $thisVal = this.val();
        this.val('').val($thisVal);
        return this;
    };
})(jQuery);
var Order = (function () {
    var windowOptions = {
        modal: true,
        width: 450,
        height: 250,
        visible: false,
        title: "Void Order",
        animation: false
    };

    var voidUrl = "";

    function allowVoid(voidAuthenticationUrl, userName, password) {
        var returnVal = false;
        $.ajax({
            async: false,
            method: "POST",
            url: voidAuthenticationUrl,
            data: {
                userName: userName,
                password: password
            },
            success: function (result) {
                returnVal = result.IsSuccess
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
        return returnVal;
    }

    return {
        getWindowOptions: function(){
            return windowOptions;
        },

        printOrder: function (e) {
            e.preventDefault();
            var orderId = $(this).data("id");
            var reportPreviewUrl = $("#reportPreviewUrl").val();
            reportPreviewUrl += "?Report=OrderReceipt&OrderId=" + orderId;
            kendoPrint(reportPreviewUrl);
        },

        printKitchen: function(e) {
            e.preventDefault();
            var orderId = $(this).data("id");
            var reportPreviewUrl = $("#reportPreviewUrl").val();
            reportPreviewUrl += "?Report=KitchenReceipt&OrderId=" + orderId;
            kendoPrint(reportPreviewUrl);
        },

        voidOrder: function(e) {
            e.preventDefault();
            voidUrl = $(this).data("voidurl");
            $("#voidAuthentication").html(Quickafe.loadingTemplate);
            $("#voidAuthentication").getKendoWindow().refresh({
                url: voidUrl
            }).center().open();
        },

        authenticateVoid: function(e){
            e.preventDefault();
            var userName = $("#UserName").val();
            var password = $("#Password").val();
            var voidAuthenticationUrl = $("#VoidAuthenticationUrl").val();
            if (allowVoid(voidAuthenticationUrl, userName, password)) {
                var voidurl = $("#VoidUrl").val();
                $.ajax({
                    url: voidurl,
                    method: "POST",
                    data: {
                        orderId: $("#VoidOrderId").val(),
                        userName: userName
                    },
                    success: function (result) {
                        if (result.IsSuccess) {
                            $("#voidAuthentication").getKendoWindow().close();
                            $("#grid").data("kendoGrid").dataSource.read();
                            $("#grid").data("kendoGrid").refresh();
                        }
                        else {
                            bootbox.alert(result.Message);
                        }
                    },
                    error: function (err) {
                        alert(err.statusText);
                    }
                });
            }
            else {
                bootbox.alert("Void is not allowed for current user name");
            }
        },

        closeVoidWindow: function() {
            $("#voidAuthentication").getKendoWindow().close();
        },

        back: function (e) {
            e.preventDefault();
            var referrer = $(this).data("referrer");
            document.location.href = referrer;
        },
    }
})();

$(function () {
    $("#grid").on("click", ".printOrder", Order.printOrder);
    $("#grid").on("click", ".printKitchen", Order.printKitchen);
    $("#grid").on("click", ".voidOrder", Order.voidOrder);
    $("#voidAuthentication").on("click", "#btnClose", Order.closeVoidWindow);
    $("#voidAuthentication").on("click", "#btnVoidAuth", Order.authenticateVoid);
    $("#voidAuthentication").kendoWindow(Order.getWindowOptions());
});
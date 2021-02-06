var PaymentDetail = (function () {
    var windowOptions = {
        modal: true,
        width: 450,
        height: 250,
        visible: false,
        title: "Void Payment",
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
        getWindowOptions: function () {
            return windowOptions;
        },

        back: function (e) {
            e.preventDefault();
            var referrer = $(this).data("referrer");
            document.location.href = referrer;
        },

        printPayment: function (e) {
            e.preventDefault();
            var paymentId = $(this).data("id");
            var orderId = $(this).data("orderid");
            var reportPreviewUrl = $("#reportPreviewUrl").val();
            reportPreviewUrl += "?Report=PaymentReceipt&PaymentId=" + paymentId + "&OrderId=" + orderId;
            kendoPrint(reportPreviewUrl);
        },

        closeVoidWindow: function () {
            $("#voidAuthentication").getKendoWindow().close();
        },

        voidPayment: function (e) {
            e.preventDefault();
            voidUrl = $(this).data("voidurl");
            $("#voidAuthentication").html(Quickafe.loadingTemplate);
            $("#voidAuthentication").getKendoWindow().refresh({
                url: voidUrl
            }).center().open();
        },

        authenticateVoid: function (e) {
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
                        paymentId: $("#VoidPaymentId").val(),
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
    }
})();

$(function () {
    $("#grid").on("click", ".printPayment", PaymentDetail.printPayment);
    $("#grid").on("click", ".voidPayment", PaymentDetail.voidPayment);
    $("#voidAuthentication").on("click", "#btnClose", PaymentDetail.closeVoidWindow);
    $("#voidAuthentication").on("click", "#btnVoidAuth", PaymentDetail.authenticateVoid);
    $("#voidAuthentication").kendoWindow(PaymentDetail.getWindowOptions());

});
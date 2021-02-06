var Payment = (function () {
    var windowOptions = {
        modal: true,
        width: 650,
        height: 350,
        visible: false,
        title: false,
        animation: false
    };

    var currentServiceUrl = "";
    var validationOptions = {
        rules: {
            amountGreaterThanZero: function (input) {
                if ($(input).attr("id") === "Amount" && input.val() != "") {
                    return input.val() > 0;
                }
                return true;
            }
        },
        messages: {
            amountGreaterThanZero: "Amount must be greater than zero"
        }
    };

    var formatMoney = "###,##0";

    return {
        getWindowOptions: function () {
            return windowOptions;
        },

        addNewClick: function (e) {
            e.preventDefault();
            currentServiceUrl = $(e.target).data("createurl");
            $("#paymentDetailWindow").html(Quickafe.loadingTemplate);
            $("#paymentDetailWindow").data("kendoWindow").refresh({
                url: currentServiceUrl
            }).center().open();
        },

        editClick: function (e) {
            e.preventDefault();
            currentServiceUrl = $("#gridPayment").data("editurl");
            $("#paymentDetailWindow").html(Quickafe.loadingTemplate);
            $("#paymentDetailWindow").data("kendoWindow").refresh({
                url: currentServiceUrl,
                data: {
                    detailUid: $(e.target).closest("a").attr("data-id")
                }
            }).center().open();
        },

        closeClick: function (e) {
            e.preventDefault();
            $("#paymentDetailWindow").data("kendoWindow").close();
        },

        deleteClick: function (e) {
            e.preventDefault();
            var data = [];
            $("input[name='chkDelete']:checked").each(function () {
                data.push($(this).val());
            });
            $.ajax({
                type: "POST",
                url: $(e.target).data("deleteurl"),
                data: {
                    arrayOfId: data
                },
                success: function (result) {
                    if (result.IsSuccess) {
                        Quickafe.refreshGrid("#gridPayment");
                        $("#totalPayment").text(result.Data.TotalPayment);
                    } else {
                        alert(result.Message);
                    }
                },
                error: function (result) {
                    alert(result.statusText);
                }
            });
        },

        cancelPayment: function(e){
            e.preventDefault();
            if (confirm('Are you sure want to cancel this payment creation ?')) {
                if (document.referrer != undefined) {
                    document.location.href = document.referrer;
                } else {
                    history.go(-1);
                }
            }
        },

        confirmPayment: function(e){
            e.preventDefault();
            if(Payment.validatePayment()) {
                $("#createPaymentForm").submit();
            }
        },

        validatePayment: function () {
            return confirm("Confirm this payment ?");
        },

        gridPaymentDataBound: function () {
            $("#totalPayment").text("Calculating...");
            $.ajax({
                type: "POST",
                url: $("#TotalPaymentUrl").val(),
                data: {
                },
                success: function (result) {
                    var totalPayment = kendo.toString(result, formatMoney);
                    $("#totalPayment").text(totalPayment);
                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        },        

        saveClick: function (e) {
            e.preventDefault();

            var validator = $("#paymentDetailWindow").kendoValidator(validationOptions).data("kendoValidator");
            if (validator.validate()) {
                $("#btnSaveDetail").addClass("k-state-disabled");
                $.ajax({
                    type: "POST",
                    url: currentServiceUrl,
                    data: {
                        uid: $("#Uid").val(),
                        paymentTypeId: $("#PaymentTypeId").data("kendoDropDownList").value(),
                        paymentTypeName: $("#PaymentTypeId").data("kendoDropDownList").text(),
                        amount: $("#Amount").data("kendoNumericTextBox").value(),
                        cardNo: $("#CardNo").val(),
                        notes: $("#Notes").val()
                    },
                    success: function (result) {
                        Quickafe.refreshGrid("#gridPayment");
                        $("#totalPayment").text(result.Data.TotalPayment);
                        $("#paymentDetailWindow").data("kendoWindow").close();
                    },
                    error: function (err) {
                        alert(err.statusText);
                        $("#btnSaveDetail").removeClass("k-state-disabled");
                    }
                });
            }
        },
    }
})();

$(function () {
    $("#paymentDetailWindow").kendoWindow(Payment.getWindowOptions());
    $("#btnAddNew").click(Payment.addNewClick);
    $("#btnDelete").click(Payment.deleteClick);
    $("#btnSave").click(Payment.saveClick);
    $("#btnConfirmPayment").click(Payment.confirmPayment);
    $("#btnCancelPayment").click(Payment.cancelPayment);
    $("#gridPayment").on("click", ".editRow", Payment.editClick);
});
var OrderDetail = (function () {
    var windowOptions = {
        modal: true,
        width: 600,
        height: 380,
        visible: false,
        title: false,
        animation: false
    };

    var windowBrowseOptions = {
        modal: true,
        width: 800,
        height: 550,
        visible: false,
        animation: false
    }

    var currentServiceUrl = "";
    var validationOptions = {};

    return {
        getWindowOptions: function () {
            return windowOptions;
        },

        getWindowBrowseOptions: function() {
            return windowBrowseOptions;
        },

        gridDataBound: function(e) {
            var grid = $("#grid").data("kendoGrid");
            grid.hideColumn(8); // hide column Notes
            
            var rows = this.dataSource.view();
            for (var rowIndex = 0; rowIndex < rows.length; rowIndex++) {
                var row = rows[rowIndex];
                var tr = $("#grid").find("[data-uid='" + row.uid + "']");
                tr.attr("title", row.Notes);
            }
        },

        ProductCodeBlur: function(e) {
            e.preventDefault();
            var unitPriceMode = $("#UnitPriceMode").val();
            if (unitPriceMode === undefined)
                unitPriceMode = 1;
            if ($("#ProductCode").val().trim().length > 0) {
                $.ajax({
                    type: "GET",
                    url: $(e.target).data("productlookup"),
                    data: {
                        ProductCode: $("#ProductCode").val()
                    },
                    success: function (result) {
                        if (result.IsSuccess && result.Data != null) {
                            var unitPrice = 0;
                            $("#ProductCode").val(result.Data.Code);
                            $("#ProductId").val(result.Data.Id);
                            $("#ProductName").val(result.Data.Name);

                            if (unitPriceMode == 1)
                                unitPrice = result.Data.UnitPrice;
                            else if (unitPriceMode == 2)
                                unitPrice = result.Data.UnitPrice2;
                            else if (unitPriceMode == 3)
                                unitPrice = result.Data.UnitPrice3;
                            else
                                unitPrice = 0;
                            $("#UnitPrice").data("kendoNumericTextBox").value(unitPrice);
                        }
                        else {
                            alert("Product is not found");
                        }
                    },
                    error: function (result) {
                        alert(result.statusText);
                    }
                });
            }
        },

        addNewClick: function (e) {
            e.preventDefault();
            var validator = $("#addEditForm").kendoValidator(validationOptions).data("kendoValidator");
            if (validator.validate()) {
                currentServiceUrl = $(e.target).data("createurl");
                $("#addEditWindow").html(Quickafe.loadingTemplate);
                $("#addEditWindow").data("kendoWindow").refresh({
                    url: currentServiceUrl
                }).center().open();
            }
        },

        editClick: function (e) {
            e.preventDefault();
            currentServiceUrl = $("#grid").data("editurl");
            $("#addEditWindow").html(Quickafe.loadingTemplate);
            $("#addEditWindow").data("kendoWindow").refresh({
                url: currentServiceUrl,
                data: {
                    detailUid: $(e.target).closest("a").attr("data-id")
                }
            }).center().open();
        },

        closeClick: function (e) {
            e.preventDefault();
            $("#addEditWindow").data("kendoWindow").close();
        },

        deleteClick: function (e) {
            e.preventDefault();
            var data = [];
            $("input[name='chkDelete']:checked").each(function () {
                data.push($(this).val());
            });
            if (data.length > 0) {
                $.ajax({
                    type: "POST",
                    url: $(e.target).data("deleteurl"),
                    data: {
                        arrayOfId: data
                    },
                    success: function (result) {
                        if (result.IsSuccess) {
                            Quickafe.refreshGrid();
                        } else {
                            alert(result.Message);
                        }
                    },
                    error: function (result) {
                        alert(result.statusText);
                    }
                });
            }
        },

        detailEntryIsValid: function(){
            if ($("#ProductCode").val().trim() === "") {
                kendoAlert("Product must be selected to create order");
                $("#ProductCode").focusEnd();
                return false;
            }
            if ($("#Qty").val() == 0) {
                kendoAlert("Quantity must be greater than zero");
                $("#Qty").focusEnd();
                return false;
            }

            return true;
        },

        saveClick: function (e) {
            e.preventDefault();

            var validator = $("#addEditForm").kendoValidator(validationOptions).data("kendoValidator");
            if (validator.validate() && OrderDetail.detailEntryIsValid()) {
                $("#btnSaveDetail").addClass("k-state-disabled");
                $.ajax({
                    type: "POST",
                    url: currentServiceUrl,
                    data: $("#addEditDetailForm").serialize(),
                    success: function (result) {
                        Quickafe.refreshGrid();
                        $("#addEditWindow").data("kendoWindow").close();
                    },
                    error: function (err) {
                        alert(err.statusText);
                        $("#btnSaveDetail").removeClass("k-state-disabled");
                    }
                });
            }
        },

        disableKeyDown: function(e) {
            if (e.keyCode != 9)
                e.preventDefault();
        },

        browseProductClick: function (e) {
            e.preventDefault();
            var unitPriceMode = $("#UnitPriceMode").val();
            var browseUrl = $("#windowBrowseProduct").data("browseurl") + "&unitPriceMode=" + unitPriceMode;
            $("#windowBrowseProduct").html(Quickafe.loadingTemplate);
            $("#windowBrowseProduct").data("kendoWindow").refresh({
                url: browseUrl
            }).title("Browse Products").center().open();
        }
    }
})();

var Order = (function () {
    var validationOptions = {
    };

    return {
        saveClick: function (e) {
            e.preventDefault();
            var validator = $("#addEditForm").kendoValidator(validationOptions).data("kendoValidator");
            if (validator.validate()) {
                $.ajax({
                    url: $(e.target).data("validateurl"),
                    type: "POST",
                    data: $("#addEditForm").serialize(),
                    async: false,
                    success: function (response) {
                        if(response.IsSuccess){
                            $("#btnSave").addClass("k-state-disabled");
                            $("#addEditForm").submit();
                        }
                        else {
                            bootbox.alert(response.Message);
                        }
                    }
                });
                
            }
        }
    }
})();

$(function () {
    $("#addEditWindow").kendoWindow(OrderDetail.getWindowOptions());
    $("#btnAddNew").click(OrderDetail.addNewClick);
    $("#btnDelete").click(OrderDetail.deleteClick);
    $("#btnSave").click(Order.saveClick);
    $("#grid").on("click", ".editRow", OrderDetail.editClick);
    $("#OrderNo").bind("keydown", OrderDetail.disableKeyDown);
    $("#windowBrowseProduct").kendoWindow(OrderDetail.getWindowBrowseOptions());
    $("body").on("click", "#btnBrowseProduct", OrderDetail.browseProductClick);
});
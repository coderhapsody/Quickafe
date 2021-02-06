var InventoryInDetail = (function () {
    var windowOptions = {
        modal: true,
        width: 600,
        height: 400,
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

        getInventoryType: function() {
            return {
                inventoryType: $("#InventoryType").getKendoDropDownList().value()
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
                    id: $(e.target).closest("a").attr("data-id")
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
        },

        detailEntryIsValid: function(){
            if ($("#ProductCode").val().trim() === "") {
                kendoAlert("Product must be selected to create order");
                $("#ProductCode").focusEnd();
                return false;
            }
            if ($("#Qty").val() <= 0) {
                kendoAlert("Quantity must be greater than zero");
                $("#Qty").focusEnd();
                return false;
            }

            return true;
        },

        saveClick: function (e) {
            e.preventDefault();

            var validator = $("#addEditDetailForm").kendoValidator(validationOptions).data("kendoValidator");
            if (validator.validate() && InventoryInDetail.detailEntryIsValid()) {
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

        browseProductClick: function (e) {
            e.preventDefault();
            var inventoryType = $("#InventoryType").getKendoDropDownList().value();
            $("#windowBrowseProduct").html(Quickafe.loadingTemplate);
            $("#windowBrowseProduct").data("kendoWindow").refresh({
                url: $("#windowBrowseProduct").data("browseurl") + "&InventoryType=" + inventoryType
            }).title("Browse Products").center().open();
        }
    }
})();

var Inventory = (function () {
    var validationOptions = {};
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
                            alert(response.Message);
                        }
                    }
                });
                
            }
        }
    }
})();

$(function () {
    $("#addEditWindow").kendoWindow(InventoryInDetail.getWindowOptions());
    $("#btnAddNew").click(InventoryInDetail.addNewClick);
    $("#btnDelete").click(InventoryInDetail.deleteClick);
    $("#btnSave").click(Inventory.saveClick);
    $("#grid").on("click", ".editRow", InventoryInDetail.editClick);
    $("#windowBrowseProduct").kendoWindow(InventoryInDetail.getWindowBrowseOptions());
    $("body").on("click", "#btnBrowseProduct", InventoryInDetail.browseProductClick);
});
var ProductCategory = (function () {
    var windowOptions = {
        modal: true,
        width: 600,
        height: 250,
        visible: false,
        title: false,
        animation: false
    };

    var currentServiceUrl = "";
    var validationOptions = {};

    return {
        getWindowOptions: function () {
            return windowOptions;
        },

        addNewClick: function (e) {
            e.preventDefault();
            currentServiceUrl = $(e.target).data("createurl");
            $("#addEditWindow").html(Quickafe.loadingTemplate);
            $("#addEditWindow").data("kendoWindow").refresh({
                url: currentServiceUrl
            }).center().open();
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

        saveClick: function (e) {
            e.preventDefault();

            var validator = $("#addEditForm").kendoValidator(validationOptions).data("kendoValidator");
            if (validator.validate()) {
                $("#btnSave").addClass("k-state-disabled");
                $.ajax({
                    type: "POST",
                    url: currentServiceUrl,
                    data: $("#addEditForm").serialize(),
                    success: function (result) {
                        if (result.IsSuccess) {
                            Quickafe.refreshGrid();
                            $("#addEditWindow").data("kendoWindow").close();
                        }
                        else {
                            kendoAlert(result.Message);
                            $("#btnSave").removeClass("k-state-disabled");
                        }
                    },
                    error: function (err) {
                        alert(err.statusText);
                        $("#btnSave").removeClass("k-state-disabled");
                    }
                });
            }
        }
    }
})();

$(function () {
    $("#addEditWindow").kendoWindow(ProductCategory.getWindowOptions());
    $("#btnAddNew").click(ProductCategory.addNewClick);
    $("#btnDelete").click(ProductCategory.deleteClick);
    $("#grid").on("click", ".editRow", ProductCategory.editClick);
});
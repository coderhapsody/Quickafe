var InventoryOut = (function () {
    var windowOptions = {
        modal: true,
        width: 650,
        height: 400,
        visible: false,
        title: false,
        animation: false
    };

    var currentServiceUrl = "";

    var validationOptions = {
    };

    return {
        getWindowOptions: function () {
            return windowOptions;
        },

        postClick: function(e) {
            e.preventDefault();
            $("#btnPosting").attr("disabled", "disabled");
            $("#btnPosting").addClass("k-state-disabled");
            var data = [];
            $("input[name='chkDelete']:checked").each(function () {
                data.push($(this).val());
            });
            $.ajax({
                type: "POST",
                url: $(e.target).data("postingurl"),
                data: {
                    arrayOfId: data
                },
                success: function (result) {
                    if (result.IsSuccess) {
                        Quickafe.refreshGrid();
                        bootbox.alert("Posting completed.");
                    } else {
                        alert(result.Message);
                    }
                    $("#btnPosting").removeClass("k-state-disabled");
                    $("#btnPosting").removeAttr("disabled");
                },
                error: function (result) {
                    alert(result.statusText);
                    $("#btnPosting").removeClass("k-state-disabled");
                    $("#btnPosting").removeAttr("disabled");
                }
            });
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
        }

    }
})();

$(function () {
    $("#btnDelete").click(InventoryOut.deleteClick);
    $("#btnPosting").click(InventoryOut.postClick);
});
var ChangePassword = (function () {
    var validationOption = {
        rules: {
            checkConfirmPassword: function (input) {
                if ($(input).attr("id") === "ConfirmPassword") {
                    return $(input).val() === $("#NewPassword").val();
                }

                return true;
            }
        },

        messages: {
            checkConfirmPassword: "Invalid password confirmation"
        }
    };

    return {
        confirmChangePassword: function (e) {
            e.preventDefault();
            var validator = $("#ChangePasswordForm").kendoValidator(validationOption).data("kendoValidator");
            if (validator.validate()) {
                $("#ChangePasswordForm").submit();
            }
        }
    }
})();

$(function () {
    
    $("#btnSubmit").click(ChangePassword.confirmChangePassword);
});
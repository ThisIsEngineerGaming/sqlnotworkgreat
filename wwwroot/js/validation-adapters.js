// Клієнтські адаптери jQuery Unobtrusive Validation
// для власних (кастомних) атрибутів валідації Data Annotations:
//   - NotFutureYearAttribute  -> data-val-notfutureyear
//   - ValidImageUrlAttribute  -> data-val-validimageurl
//
// Файл підключається один раз через _ValidationScriptsPartial.cshtml,
// одразу після jquery.validate.unobtrusive.js
(function ($) {
    if (!$ || !$.validator || !$.validator.unobtrusive) {
        return;
    }

    // ---- NotFutureYear ---------------------------------------------------
    $.validator.addMethod("notfutureyear", function (value, element, params) {
        if (this.optional(element)) {
            return true;
        }
        var year = parseInt(value, 10);
        return !isNaN(year) && year >= params.min && year <= params.max;
    });

    $.validator.unobtrusive.adapters.add(
        "notfutureyear",
        ["min", "max"],
        function (options) {
            options.rules["notfutureyear"] = {
                min: parseInt(options.params.min, 10),
                max: parseInt(options.params.max, 10)
            };
            options.messages["notfutureyear"] = options.message;
        }
    );

    // ---- ValidImageUrl -----------------------------------------------------
    $.validator.addMethod("validimageurl", function (value, element) {
        if (this.optional(element)) {
            return true;
        }
        try {
            var url = new URL(value);
            return url.protocol === "http:" || url.protocol === "https:";
        } catch (e) {
            return false;
        }
    });

    $.validator.unobtrusive.adapters.addBool("validimageurl");
})(window.jQuery);

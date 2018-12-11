(function (app) {
    app.filter("statusFilter",
        function() {
            return function(input) {
                if (input === true)
                    return "Đã kích hoạt";
                else
                    return "Chưa kích hoạt";
            }
        });
    app.filter("cancelOrderFilter",
        function () {
            return function (input) {
                if (input === true)
                    return "Đã hủy đơn hàng";
                else
                    return "Đã đặt hàng";
            }
        });
})(angular.module("uStora.common"));
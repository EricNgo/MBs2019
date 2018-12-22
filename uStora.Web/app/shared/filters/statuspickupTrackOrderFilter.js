(function (app) {
    app.filter('statuspickupTrackOrderFilter', function () {
        return function (input) {
            if (input == true)
                return "Đã lấy hàng";
            else
                return "Chưa lấy hàng";
        }
    })
})(angular.module('uStora.common'));
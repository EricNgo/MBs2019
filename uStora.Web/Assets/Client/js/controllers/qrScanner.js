$(function () {
    var app = new Vue({
        el: '#app',
        data: {
            scanner: null,
            activeCameraId: null,
            cameras: [],
            scans: []
        },
        mounted: function () {
            var self = this;
            self.scanner = new Instascan.Scanner({ video: document.getElementById('preview'), scanPeriod: 5 });
            self.scanner.addListener('scan', function (content, image) {
                self.scans.unshift({ date: +(Date.now()), content: content });
                var qrCode = $("#qrCode").val();
                var IsEnoughCoin = $("#IsEnoughCoin").val();
                console.log(qrCode);
                
                if (content === qrCode) {
                    toastr.success("Mã QR hợp lệ");
                    if (IsEnoughCoin === "True") {
                        console.log(IsEnoughCoin === true);
                        window.location.href = "/qr-order.htm";
                    }
                    else {
                        toastr.error("Bạn không đủ tiền để thực hiện giao dịch! Vui lòng nạp tiền.");
                    }
                        
                } else {
                    toastr.error("Mã QR không hợp lệ hoặc bạn chưa tạo.");
                }
            });
            Instascan.Camera.getCameras().then(function (cameras) {
                self.cameras = cameras;
                if (cameras.length > 0) {
                    self.activeCameraId = cameras[0].id;
                    self.scanner.start(cameras[0]);
                } else {
                    console.error('No cameras found.');
                }
            }).catch(function (e) {
                console.error(e);
            });
        },
        methods: {
            formatName: function (name) {
                return name || '(unknown)';
            },
            selectCamera: function (camera) {
                this.activeCameraId = camera.id;
                this.scanner.start(camera);
            }
        }
    });
});
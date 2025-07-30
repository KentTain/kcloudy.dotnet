var cfca = function () {
    var loadCFCAInfoUrl, subjectDNFilter, serialNumFilter,ssoDomain, CryptoAgent;
    return {
        config: function () {

        },
        init: function (loadCFCAInfoUrl,ssoDomain) {
            this.loadCFCAInfoUrl = loadCFCAInfoUrl;
            this.ssoDomain = ssoDomain;
            var _this = $(this);　　
            $.get(this.loadCFCAInfoUrl, function (data) {
                if (data && data.result && data.result.CompanyName) {
                    _this[0].subjectDNFilter = data.result.CertificateSubject;
                    _this[0].serialNumFilter = data.result.SerialNumber;
                }
            });
        },
        initOperatingEnvironment: function () {
            if (!this.checkBrowser())
                return false;
            this.initCryptoKit();
            if (!this.selectCertificate())
                return false;
            return true;
        },
        checkBrowser: function () {
            if (navigator.appName.indexOf("Internet") == -1 && navigator.appVersion.indexOf("Trident") == -1) {
                $.messager.alert('此功能请在IE浏览器或支持IE内核浏览器(如QQ浏览器、360浏览器等)中使用！', 'info');
                return false;
            }
            return true;
        },
        initCryptoKit: function () {
            try {
                var eDiv = document.createElement("div");
                if (window.navigator.cpuClass == "x86") {
                    eDiv.innerHTML = "<object id=\"CryptoAgent\" codebase=\"" + this.ssoDomain + "/Resources/Cryptokit/x86/CryptoKit.Ultimate.x86.cab\" classid=\"clsid:4C588282-7792-4E16-93CB-9744402E4E98\" ></object>";
                }
                else {
                    eDiv.innerHTML = "<object id=\"CryptoAgent\" codebase=\"" + this.ssoDomain + "/Resources/Cryptokit/x64/CryptoKit.Ultimate.x64.cab\" classid=\"clsid:B2F2D4D4-D808-43B3-B355-B671C0DE15D4\" ></object>";
                }
                document.body.appendChild(eDiv);
            }
            catch (e) {
                return;
            }
            this.CryptoAgent = document.getElementById("CryptoAgent");
        },
        selectCertificate: function () {
            var bSelectCertResult = '';
            try {
                if ((!this.serialNumFilter && !this.subjectDNFilter) || (this.serialNumFilter.length === 0 && this.subjectDNFilter.length === 0)) {
                    $.messager.alert('还未进行uKey认证！', 'info');
                    return false;
                }
                bSelectCertResult = this.CryptoAgent.SelectCertificate(this.subjectDNFilter, '', this.serialNumFilter);
                if (!bSelectCertResult) {
                    $.messager.alert('没有找到匹配的证书，请确认是否正确插入Ukey！', 'info');
                    return false;
                }
            }
            catch (e) {
                try {
                    var errorDesc = this.CryptoAgent.GetLastErrorDesc();
                    $.messager.alert(errorDesc, 'info');
                } catch (ex) {
                    $.messager.alert('请安装插件！', 'info');
                }
                return false;
            }
            return true;
        },
        signVoucher: function (source) {
            if (!this.selectCertificate())
                return false;
            try {
                return this.CryptoAgent.SignMsgPKCS1('create_ious' +source, 'SHA-1');
            } catch (e) {
                try {
                    return this.CryptoAgent.SignMsgPKCS1('create_ious' +source, 'SHA-256');
                } catch (ex) {
                    $.messager.alert('签名失败！', 'error');
                }
            }
            return '';
        }
    }
}();

function showAgreement() {
    showMaxDialog({ title: '信用凭证（财富共赢）服务协议', href: '/Agreement/WhiteBar',border:false });
}
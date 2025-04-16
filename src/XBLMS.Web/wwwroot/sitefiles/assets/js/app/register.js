var $url = '/register';
var $urlCaptcha = '/login/captcha';

var data = utils.init({
  pageAlert: null,
  captchaToken: null,
  captchaUrl: null,
  isUserCaptchaDisabled: false,
  form: {
    userName: null,
    displayName: null,
    employeeId: null,
    mobile: null,
    email: null,
    password: null,
    confirmPassword: null,
    captchaValue: null,
  },
  version: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.version = res.version;
      $this.isUserCaptchaDisabled = res.isUserCaptchaDisabled;
      if ($this.isUserCaptchaDisabled) {
        $this.btnTypeClick();
      } else {
        $this.apiCaptcha();
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCaptcha: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptcha).then(function (response) {
      var res = response.data;

      $this.captchaToken = res.value;
      $this.captchaUrl = $apiUrl + $urlCaptcha + '?token=' + $this.captchaToken;
      $this.btnTypeClick();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      appRegister: true,
      userName: this.form.userName,
      displayName: this.form.displayName,
      employeeId: this.form.employeeId,
      mobile: this.form.mobile,
      email: this.form.email,
      password: this.form.password ? md5(this.form.password) : '',
      token: this.captchaToken,
      value: this.form.captchaValue
    }).then(function (response) {
      var res = response.data;

      $this.pageAlert = {
        type: 'success',
        title: '注册成功，请登录'
      };

      setTimeout(function () {
        location.href = '/app/login';
      }, 1500);
    }).catch(function (error) {
      utils.notifyError(error);
      $this.apiCaptcha();
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnTypeClick: function () {
    var $this = this;

    this.$refs.formRegister && this.$refs.formRegister.clearValidate();
    setTimeout(function () {
      $this.$refs['userName'].focus();
    }, 100);
  },

  btnCaptchaClick: function () {
    this.apiCaptcha();
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.formRegister.validate(function (valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  validateConfirmPassword: function (rule, value, callback) {
    if (value !== this.form.password) {
      callback(new Error('两次输入密码不一致'));
    } else {
      callback();
    }
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    document.title = DOCUMENTTITLECN;
    this.apiGet();
  }
});

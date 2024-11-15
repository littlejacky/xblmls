var $url = '/index';


var data = utils.init({
  appMenuActive:"index",
  rightUrl: utils.getRootUrl('dashboard')
});

var methods = {
  btnAppMenuClick: function (common) {

    var $this = this;

    $this.appMenuActive = common;

    if (common === 'index') {
      document.title = '首页';
      $this.rightUrl = utils.getRootUrl('dashboard');
    }
    if (common === 'studyPlan') {
      document.title = '学习中心';
      $this.rightUrl = utils.getStudyUrl("studyPlan");
    }
    if (common === 'studyCourse') {
      document.title = '课程中心';
      $this.rightUrl = utils.getStudyUrl("studyCourse");
    }
    if (common === 'exam') {
      document.title = '考试中心';
      $this.rightUrl = utils.getExamUrl("examPaper");
    }
    if (common === 'mine') {
      document.title = '用户中心';
      $this.rightUrl = utils.getRootUrl('mine');
    }

    $this.$nextTick(() => {
      $this.$refs.homeRightIframe.src = $this.rightUrl;
    })

  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.loading(this, false);
    document.title = '首页';
    this.btnAppMenuClick("index");
  }
});

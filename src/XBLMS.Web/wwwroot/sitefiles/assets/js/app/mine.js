var $url = '/index';


var data = utils.init({
  user: null,
  appMenuActive: "mine",
  courseList:null
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;
      $this.courseList = res.courseList;
      if (res.user) {
        $this.user = res.user;
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnAppMenuClick: function (common) {
    if (common === 'index') {
      location.href = utils.getIndexUrl();
    }
    if (common === 'studyPlan') {
      location.href = utils.getStudyUrl("studyPlan");
    }
    if (common === 'studyCourse') {
      location.href = utils.getStudyUrl("studyCourse");
    }
    if (common === 'exam') {
      location.href = utils.getExamUrl("examPaper");
    }
    if (common === 'wenjuan') {
      location.href = utils.getExamUrl("examQuestionnaire");
    }
    if (common === 'mine') {
      location.href = utils.getRootUrl('mine');
    }
  },
  btnViewCourseClick: function (row) {

    var curl = utils.getStudyUrl('studyCourseInfo', { id: row.id, planId: row.planId });
    if (row.offLine) {
      curl = utils.getStudyUrl('studyCourseOfflineInfo', { id: row.id, planId: row.planId });
    }

    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: curl,
      width: "100%",
      height: "100%",
      end: function () {
        $this.apiGet();
      }
    });
  },
  btnTab: function (common) {

    var $this = this;

    if (common === 'info') {
      top.utils.openLayer({
        title: false,
        closebtn: 0,
        url: utils.getRootUrl('profile'),
        width: "100%",
        height: "100%",
        end: function () {
          $this.apiGet();
        }
      });
    }
    if (common === 'pwd') {
      top.utils.openLayer({
        title: false,
        closebtn: 0,
        url: utils.getRootUrl('password'),
        width: "100%",
        height: "100%"
      });
    }
    if (common === 'courseLog') {
      location.href = utils.getStudyUrl('studyCourseLog');
    }
    if (common === 'logout') {
      location.href = utils.getRootUrl("logout");
    }
    if (common === 'shuati') {
      location.href = utils.getExamUrl("examPracticeLog");
    }
    if (common === 'cer') {
      location.href = utils.getExamUrl("examPaperCer");
    }
    if (common === 'score') {
      location.href = utils.getExamUrl("examPaperScore");
    }
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    document.title = "我的";
    this.apiGet();
  }
});

var $url = '/index';


var data = utils.init({
  user: null,
  appMenuActive: "mine",
  courseList: null,
  version:null
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.version = res.version;
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
  setDocumentTitel: function () {
    top.document.title = "我的";
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
        $this.setDocumentTitel();
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
          $this.setDocumentTitel();
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
        height: "100%",
        end: function () {
          $this.setDocumentTitel();
        }
      });
    }
    if (common === 'courseLog') {
      top.utils.openLayer({
        title: false,
        closebtn: 0,
        url: utils.getStudyUrl('studyCourseLog'),
        width: "100%",
        height: "100%",
        end: function () {
          $this.setDocumentTitel();
          $this.apiGet();
        }
      });
    }
    if (common === 'logout') {
      top.utils.openLayer({
        title: false,
        closebtn: 0,
        url: utils.getRootUrl("logout"),
        width: "100%",
        height: "100%",
        end: function () {
          $this.setDocumentTitel();
        }
      });
    }
    if (common === 'shuati') {
      top.utils.openLayer({
        title: false,
        closebtn: 0,
        url: utils.getExamUrl("examPracticeLog"),
        width: "100%",
        height: "100%",
        end: function () {
          $this.setDocumentTitel();
        }
      });
    }
    if (common === 'cer') {
      top.utils.openLayer({
        title: false,
        closebtn: 0,
        url: utils.getExamUrl("examPaperCer"),
        width: "100%",
        height: "100%",
        end: function () {
          $this.setDocumentTitel();
        }
      });
    }
    if (common === 'score') {
      top.utils.openLayer({
        title: false,
        closebtn: 0,
        url: utils.getExamUrl("examPaperScore"),
        width: "100%",
        height: "100%",
        end: function () {
          $this.setDocumentTitel();
        }
      });
    }
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.setDocumentTitel();
    this.apiGet();
  }
});

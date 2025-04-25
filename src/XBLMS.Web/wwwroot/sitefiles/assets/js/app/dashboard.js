var $url = "/dashboard";
var $practiceUrl = "/exam/examPractice/submit";

var data = utils.init({
  user: null,
  passSeries: [0],
  passChartOptions: {
    chart: {
      type: 'radialBar',
      toolbar: {
        show: false
      }
    },
    plotOptions: {
      radialBar: {
        startAngle: -135,
        endAngle: 225,
        hollow: {
          margin: 0,
          size: '70%',
          background: '#fff',
          image: undefined,
          imageOffsetX: 0,
          imageOffsetY: 0,
          position: 'front',
          dropShadow: {
            enabled: true,
            top: 3,
            left: 0,
            blur: 4,
            opacity: 0.24
          }
        },
        track: {
          background: '#fff',
          strokeWidth: '88%',
          margin: 0, // margin is in pixels
          dropShadow: {
            enabled: true,
            top: -3,
            left: 0,
            blur: 4,
            opacity: 0.35
          }
        },

        dataLabels: {
          show: true,
          name: {
            offsetY: -20,
            show: true,
            color: '#ff6a00',
            fontSize: '18px'
          },
          value: {
            formatter: function (val) {
              return parseInt(val) + '%';
            },
            color: '#000',
            fontSize: '24px',
            show: true,
          }
        }
      }
    },
    fill: {
      colors: ['#19cb98']
    },
    stroke: {
      lineCap: 'round'
    },
    labels: ['及格率'],
  },

  allPercent: 0,
  examTotal: 0,
  examPercent: 0,
  examMoniTotal: 0,
  examMoniPercent: 0,

  practiceAnswerTmTotal: 0,
  practiceAnswerPercent: 0,
  practiceAllTmTotal: 0,
  practiceAllPercent: 0,
  practiceCollectTmTotal: 0,
  practiceCollectPercent: 0,
  practiceWrongTmTotal: 0,
  practiceWrongPercent: 0,

  examPaper: null,
  examMoni: null,
  practiceArrangement: null,

  taskTotal: 0,
  taskPaperTotal: 0,
  taskQTotal: 0,
  taskAssTotal: 0,
  taskPlanTotal: 0,
  taskCourseTotal: 0,
  taskDialogVisible: false,
  taskPracticeTotal: 0,

  courseList: null,
  studyPlan: null,
  studyPlanTotalCredit: 0,
  studyPlanTotalOverCredit: 0,
  totalCourse: 0,
  totalOverCourse: 0,
  totalDuration: 0,

  topCer: null,
  dateStr: null,

  planCount: 0,
  planOverCount: 0,

  appMenuActive: "index",
  version: null
});

var methods = {
  apiGet: function () {
    var $this = this;


    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.user = res.user;

      $this.dateStr = res.dateStr;

      $this.examTotal = res.examTotal;
      $this.examPercent = res.examPercent;
      $this.examMoniTotal = res.examMoniTotal;
      $this.examMoniPercent = res.examMoniPercent;

      $this.practiceAllPercent = res.practiceAllPercent;
      $this.practiceAllTmTotal = res.practiceAllTmTotal;

      $this.practiceAnswerTmTotal = res.practiceAnswerTmTotal;
      $this.practiceAnswerPercent = res.practiceAnswerPercent;

      $this.practiceWrongTmTotal = res.practiceWrongTmTotal;
      $this.practiceWrongPercent = res.practiceWrongPercent;

      $this.practiceCollectTmTotal = res.practiceCollectTmTotal;
      $this.practiceCollectPercent = res.practiceCollectPercent;

      $this.examPaper = res.examPaper;
      $this.examMoni = res.examMoni;
      $this.examPractice = res.examPractice;

      $this.taskPaperTotal = res.taskPaperTotal;
      $this.taskQTotal = res.taskQTotal;
      $this.taskPlanTotal = res.taskPlanTotal;
      $this.taskCourseTotal = res.taskCourseTotal;
      $this.taskAssTotal = res.taskAssTotal;

      $this.taskTotal = $this.taskPaperTotal + $this.taskQTotal + $this.taskAssTotal + $this.taskPlanTotal + $this.taskCourseTotal + $this.taskPracticeTotal;

      $this.courseList = res.courseList;

      $this.studyPlan = res.studyPlan;
      $this.studyPlanTotalCredit = res.studyPlanTotalCredit;
      $this.studyPlanTotalOverCredit = res.studyPlanTotalOverCredit;

      $this.totalCourse = res.totalCourse;
      $this.totalOverCourse = res.totalOverCourse;

      $this.totalDuration = res.totalDuration;

      $this.topCer = res.topCer;

      $this.planCount = 100;
      $this.planOverCount = 100;

      $this.version = res.version;

      setTimeout(function () {
        $this.passSeries = [100];
        $this.planCount = res.planCount;
        $this.planOverCount = res.planOverCount;
      }, 1000);

      setTimeout(function () {
        $this.passSeries = [res.allPercent];
      }, 2000);

    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnViewCer: function (cer) {
    top.utils.openLayerPhoto({
      title: cer.name,
      id: cer.id,
      src: cer.cerImg + '?r=' + Math.random()
    })
  },
  btnCreatePracticeClick: function (practiceType) {
    if (practiceType === 'All') {
      if (this.practiceAllTmTotal > 0) {
        this.apiCreatePractice(practiceType);
      }
      else {
        utils.error("没有题目可以练习");
      }
    }
    if (practiceType === 'Collect') {
      if (this.practiceCollectTmTotal > 0) {
        this.apiCreatePractice(practiceType);
      }
      else {
        utils.error("没有题目可以练习");
      }
    }
    if (practiceType === 'Wrong') {
      if (this.practiceWrongTmTotal > 0) {
        this.apiCreatePractice(practiceType);
      }
      else {
        utils.error("没有题目可以练习");
      }
    }
  },
  apiCreatePractice: function (practiceType, groupId) {
    var $this = this;

    utils.loading(this, true, "正在创建练习...");

    $api.post($practiceUrl, { practiceType: practiceType }).then(function (response) {
      var res = response.data;

      if (res.success) {
        $this.goPractice(res.id);
      }
      else {
        utils.error(res.error);
      }

    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },
  goPractice: function (id) {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getExamUrl('examPracticing', { id: id }),
      width: "100%",
      height: "100%",
      end: function () {
        $this.setDocumentTitel();
        $this.apiGet();
      }
    });
  },
  goMoni: function () {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getExamUrl("examPaperMoni"),
      width: "100%",
      height: "100%",
      end: function () {
        $this.setDocumentTitel();
        $this.apiGet();
      }
    });
  },
  goShuati: function () {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getExamUrl("examPractice"),
      width: "100%",
      height: "100%",
      end: function () {
        $this.setDocumentTitel();
        $this.apiGet();
      }
    });
  },
  goWenjuan: function () {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getExamUrl("examQuestionnaire"),
      width: "100%",
      height: "100%",
      end: function () {
        $this.setDocumentTitel();
        $this.apiGet();
      }
    });
  },
  goAss: function () {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getExamUrl("examAssessment"),
      width: "100%",
      height: "100%",
      end: function () {
        $this.setDocumentTitel();
        $this.apiGet();
      }
    });
  },
  goCer: function () {
    var $this = this;
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
  },
  btnViewPaperClick: function (row) {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getExamUrl('examPaperInfo', { id: row.id, planId: row.planId, courseId: row.courseId }),
      width: "100%",
      height: "100%",
      end: function () {
        $this.setDocumentTitel();
        $this.apiGet();
      }
    });
  },
  btnViewPlanClick: function (id) {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getStudyUrl('studyPlanInfo', { id: id }),
      width: "100%",
      height: "100%",
      end: function () {
        $this.setDocumentTitel();
        $this.apiGet();
      }
    });
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
  setDocumentTitel: function () {
    top.document.title = "首页";
  },
  btnViewPracticeClick: function (res) {
    $this.goPractice(res.id);
  }
};
Vue.component("apexchart", {
  extends: VueApexCharts
});
var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.setDocumentTitel();
    this.apiGet();
  },
});

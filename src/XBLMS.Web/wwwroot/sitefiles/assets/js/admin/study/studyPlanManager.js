var $url = '/study/studyPlanManager';

var $urlUser = $url + '/user';
var $urlUserExport = $urlUser + '/export';

var $urlCourse = $url + '/course';
var $urlCourseExport = $urlCourse + '/export';


var data = utils.init({
  id: 0,
  plan: null,
  form: {
    id: 0,
    state: '',
    keyWords: '',
    pageIndex: 1,
    pageSize: PER_PAGE
  },
  userList: null,
  userTotal: 0,
  userSelection: [],
  userUpdateDateTimeDialogVisible: false,
  userUpdateDateTimeForm: {
    examBeginDateTime: '',
    examEndDateTime: ''
  },
  tabPosition: 'left',

  formCourse: {
    id: 0,
    keywords: '',
  },
  courseList: null,
  passSeries: [100],
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
            fontSize: '16px'
          },
          value: {
            formatter: function (val) {
              return val + '%';
            },
            color: '#000',
            fontSize: '36px',
            show: true,
          }
        }
      }
    },
    fill: {
      colors: ['#67C23A']
    },
    stroke: {
      lineCap: 'round'
    },
    labels: ['完成率'],
  },
  pass1Series: [100],
  pass1ChartOptions: {
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
            fontSize: '16px'
          },
          value: {
            formatter: function (val) {
              return val + '%';
            },
            color: '#000',
            fontSize: '36px',
            show: true,
          }
        }
      }
    },
    fill: {
      colors: ['#67C23A']
    },
    stroke: {
      lineCap: 'round'
    },
    labels: ['达标率'],
  },
});

var methods = {
  apiGet: function () {
    var $this = this;
    utils.loading(this, true);
    $api.get($url, { params: { id: this.id } }).then(function (response) {
      var res = response.data;
      $this.plan = res.item;

      $this.passSeries = [utils.formatPercentFloat($this.plan.totalPassUser, $this.plan.totalUser)];
      $this.pass1Series = [utils.formatPercentFloat($this.plan.totalPass1User, $this.plan.totalUser)];

    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false, { layer: true });
    });
  },
  apiGetUser: function () {
    var $this = this;
    utils.loading(this, true);
    $api.get($urlUser, { params: $this.form }).then(function (response) {
      var res = response.data;
      $this.userList = res.list;
      $this.userTotal = res.total;

    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  userHandleSelectionChange(val) {
    this.userSelection = val;
  },
  userClearSelection: function () {
    this.userSelection = [];
    this.userUpdateDateTimeDialogVisible = false;
  },
  userHandleCurrentChange: function (val) {
    this.form.pageIndex = val;
    this.apiGetUser();
  },
  btnUserSearchClick: function () {
    this.form.pageIndex = 1;
    this.apiGetUser();
  },
  btnViewClick: function (id) {
    utils.openUserView(id);
  },
  btnAdminViewClick: function (id) {
    utils.openAdminView(id);
  },
  btnUserExportClick: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlUserExport, this.form).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },


  apiGetCourse: function () {
    var $this = this;
    utils.loading(this, true);
    $api.get($urlCourse, { params: $this.formCourse }).then(function (response) {
      var res = response.data;
      $this.courseList = res.list;

    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnCourseSearchClick: function () {
    this.apiGetCourse();
  },
  btnCourseExportClick: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCourseExport, this.formCourse).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnPaperSocreView: function (id) {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getCommonUrl('examPaperUserLayerView', { id: id }),
      width: "99%",
      height: "99%"
    });
  }
};
Vue.component("apexchart", {
  extends: VueApexCharts
});
var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.id = this.form.id = this.formCourse.id = utils.getQueryInt("id");
    this.apiGet();
    this.apiGetUser();
    this.apiGetCourse();

  }
});

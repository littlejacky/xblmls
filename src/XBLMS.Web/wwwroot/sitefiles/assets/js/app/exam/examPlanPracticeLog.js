var $url = "/exam/examPlanPracticeLog";
var $urlDelete = $url + "/del";

var data = utils.init({
  form: {
    keyWords: '',
    dateFrom: '',
    dateTo: '',
    pageIndex: 1,
    pageSize: PER_PAGE
  },
  list: [],
  total: 0,
  dialogVisible:false,
  loadMoreLoading: false
});

var methods = {
  apiGet: function () {
    var $this = this;

    if (this.total === 0) {
      utils.loading(this, true);
    }
    $api.get($url, { params: this.form }).then(function (response) {
      var res = response.data;

      if (res.list && res.list.length > 0) {
        res.list.forEach(paper => {
          $this.list.push(paper);
        });
      }
      $this.total = res.total;

    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
      $this.loadMoreLoading = false;
    });
  },
  btnSearchClick: function () {
    this.form.pageIndex = 1;
    this.list = [];
    this.apiGet();
  },
  btnLoadMoreClick: function () {
    this.loadMoreLoading = true;
    this.form.pageIndex++;
    this.apiGet();
  },
  goBack: function () {
    location.href = utils.getExamUrl('examPractice');
  },
  goPracticeResult: function (id) {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getExamUrl('examPracticeResult', { id: id }),
      width: "100%",
      height: "100%",
    });
  },
  goPlanPractice: function (id) {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getExamUrl('examPlanPracticing', { id: id }),
      width: "100%",
      height: "100%",
    });
  },
  btnClearClick: function () {
    var $this = this;
    top.utils.alertWarning({
      title: '清空练习记录',
      text: '此操作将清空所有练习记录，确定吗？',
      callback: function () {
        $this.apiClear();
      }
    });
  },
  apiClear: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDelete).then(function (response) {
      var res = response.data;
      if (res.value) {
        utils.success("已清空", { layer: true });
      }
    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
      $this.apiGet();
    });
  },
  btnSearchClick: function () {
    this.dialogVisible = false;
    this.list = [];
    this.apiGet();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    top.document.title = "任务记录";
    this.apiGet();
  },
});

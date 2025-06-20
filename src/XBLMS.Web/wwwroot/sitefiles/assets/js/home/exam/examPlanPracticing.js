var $url = '/exam/examPlanPracticing';
var $urlTm = $url + '/tm';
var $urlAnswer = $url + '/answer';
var $urlCollection = $url + '/collection';
var $urlCollectionRemove = $url + '/collectionRemove';
var $urlWrongRemove = $url + '/wrongRemove';
var $urlSubmitTiming = $url + "/submitTiming";
var $urlSubmit = $url + "/submit";

var data = utils.init({
  id: utils.getQueryInt("id"),
  tmIds: [],
  tmList: [],
  total: 0,
  answerTotal: 0,
  rightTotal: 0,
  wrongTotal: 0,
  title: '',
  tm: null,
  tmIndex: 0,
  watermark: null,
  answerResult: null,
  openExist: false,
  isTiming: false,
  timingMinute: 0,
  surplusSecond: 0,
  curTimingSecond:1
});

var methods = {
  apiGet: function () {
    var $this = this;
    utils.loading(this, false);
    utils.loading(this, true, "正在加载题目...");

    $api.get($url, { params: { id: this.id } }).then(function (response) {
      var res = response.data;
      $this.tmIds = res.tmIds;
      $this.total = res.total;
      $this.title = res.title;
      $this.answerTotal = res.answerTotal;
      $this.rightTotal = res.rightTotal;
      $this.wrongTotal = res.wrongTotal;

      $this.watermark = res.watermark;
      $this.tmIndex = res.tmIndex;
      $this.openExist = res.openExist;
      $this.isTiming = res.isTiming;

      if (res.isTiming) {
        if (res.useTimeSecond > -1) {
          $this.surplusSecond = Date.now() + res.timingMinute * 60 * 1000 - res.useTimeSecond * 1000;
          if ($this.surplusSecond > 0) {
            $this.timingChange();
          }
          else {
            $this.goResult();
          }
        }
        else {
          $this.goResult();
        }
      }

      $this.apiGetTmInfo($this.tmIds[0])

    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  apiGetTmInfo: function (tmid) {
    var $this = this;
    utils.loading(this, true);

    $api.get($urlTm, { params: { id: tmid } }).then(function (response) {
      var res = response.data;
      $this.tm = res.item;
    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  answerChange: function () {

    let setTm = this.tm;
    let answer = setTm.answer;

    if (setTm.baseTx === "Duoxuanti") {
      answer = setTm.optionsValues.join('');
    }
    var completionStatus = true;
    if (setTm.baseTx === "Tiankongti") {
      answer = setTm.optionsValues.join(',');
    }

    this.tm.answer = answer;
  },
  apiSubmitAnswer: function () {
    var $this = this;
    utils.loading(this, true);

    $api.post($urlAnswer, { id: this.tm.id, answer: this.tm.answer,practiceId:this.id }).then(function (response) {
      var res = response.data;
      $this.answerResult = res;
      if ($this.answerResult.isRight) {
        $this.rightTotal++;
      }
      else {
        $this.wrongTotal++;
      }
      $this.answerTotal = $this.rightTotal + $this.wrongTotal;

    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnSubmitAnswerClick: function () {
    if (this.tm.answer && this.tm.answer.length > 0) {
      this.apiSubmitAnswer();
    }
    else {
      utils.error("请答题", { layer: true });
    }
  },
  btnDownClick: function () {
    if (this.tmIndex === this.total - 1) {
      this.btnResultClick();
    }
    else {
      this.tm = null;
      this.answerResult = null;
      this.tmIndex++;
      this.apiGetTmInfo(this.tmIds[this.tmIndex]);
    }
  },
  btnCollectionClick: function () {
    this.apiCollection();
  },
  apiCollection: function () {
    var $this = this;
    utils.loading(this, true);

    $api.post($urlCollection, { id: this.tm.id }).then(function (response) {
      var res = response.data;
      if (res.value) {
        $this.tm.isCollection = true;
        utils.success("收藏成功", { layer: true })
      }
    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnCollectionRemoveClick: function () {
    this.apiCollectionRemove();
  },
  apiCollectionRemove: function () {
    var $this = this;
    utils.loading(this, true);

    $api.post($urlCollectionRemove, { id: this.tm.id }).then(function (response) {
      var res = response.data;
      if (res.value) {
        $this.tm.isCollection = false;
        utils.success("已取消收藏", { layer: true })
      }
    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnWrongRemoveClick: function(){
    this.apiWrongRemove();
  },
  apiWrongRemove: function () {
    var $this = this;
    utils.loading(this, true);

    $api.post($urlWrongRemove, { id: this.tm.id }).then(function (response) {
      var res = response.data;
      if (res.value) {
        $this.tm.isWrong = false;
        utils.success("已移出错题库", { layer: true })
      }
    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnResultClick: function () {
    this.goResult();
  },
  goResult: function () {
    this.apiSubmit();
    utils.loading(this, true,"正在统计练习...");
    location.href = utils.getExamUrl("examPlanPracticeResult", { id: this.id });
  },
  apiSubmit: function () {
    $api.post($urlSubmit, { id: this.startId }).then(function (response) { });
  },
  apiSubmitTiming: function () {
    $api.post($urlSubmitTiming, { id: this.startId }).then(function (response) { });
  },
  timingFinish: function () {
    this.goResult();
  },
  timingChange: function () {
    if (this.curTimingSecond % 5 === 0) {
      this.apiSubmitTiming();
    }
    var $this = this;
    setTimeout(function () {
      $this.curTimingSecond++;
      $this.timingChange();
    }, 1000)
  }
};
var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});

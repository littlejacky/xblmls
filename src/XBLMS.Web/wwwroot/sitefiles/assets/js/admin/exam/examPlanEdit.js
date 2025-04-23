var $url = 'exam/examPlanEdit';
var $urlGetConfig = $url + '/getConfig';

var data = utils.init({
  id: utils.getQueryInt('id'),
  copyId: utils.getQueryInt('copyId'),
  treeId: utils.getQueryInt('treeId'),
  item: null,
  userGroupList: null,
  tmGroupList: null,
  tmAllGroupList: null,
  tmFixedGroupList: null,
  paperTree: null,
  cerList: null,
  txList: null,
  tmRandomConfig: [],
  selectTms: null,
  form: null,
  txTotalScore: 0,
  submitDialogVisible: false,
  submitSubmitType: 'Save',
  submitSubmitIsClear: false,
  tmConfigDialogVisible: false,
  isUpdateDateTime: false,
  isUpdateExamTimes: false,
  totalPercentage: 0,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, { params: { id: this.id } }).then(function (response) {
      var res = response.data;

      $this.tmAllGroupList = res.tmGroupList;
      $this.tmFixedGroupList = res.tmFixedGroupList;
      $this.txList = res.txList;
      $this.userGroupList = res.userGroupList;
      $this.paperTree = res.paperTree;
      $this.cerList = res.cerList;

      $this.tmGroupList = $this.tmAllGroupList;


      $this.form = _.assign({}, res.item);
      if ($this.id > 0) {
        if (res.configList != null && res.configList.length > 0) {
          $this.tmRandomConfig = res.configList;
        }
        if ($this.copyId > 0) {
          $this.id = 0;
          $this.form.id = 0;
          $this.form.title = $this.form.title + "-复制";
          $this.form.submitType = "Save";
        }

        // 确保题目组占比数据正确初始化
        if (!$this.form.tmGroupPercentages) {
          $this.form.tmGroupPercentages = {};
        }

        // 如果有题目组但没有对应的占比数据，则初始化占比
        if ($this.form.tmGroupIds && $this.form.tmGroupIds.length > 0) {
          var totalGroups = $this.form.tmGroupIds.length;
          var defaultPercentage = Math.floor(100 / totalGroups);

          $this.form.tmGroupIds.forEach(function (groupId) {
            if (!$this.form.tmGroupPercentages[groupId]) {
              $this.$set($this.form.tmGroupPercentages, groupId, defaultPercentage);
            }
          });

          // 计算总占比
          $this.calculateTotalPercentage();
        }
      }
      else {
        if ($this.treeId > 0) {
          $this.form.treeId = $this.treeId;
        }
        else {
          $this.form.treeId = null;
        }

        $this.$nextTick(() => {
          $this.apiGetConfig();
        })

      }


      if ($this.form.cerId === 0) {
        $this.form.cerId = null;
      }

    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  apiGetConfig: function () {
    var $this = this;

    utils.loading(this, true, "正在加载题目配置数据");
    $api.post($urlGetConfig, { txIds: $this.form.txIds, tmGroupIds: $this.form.tmGroupIds }).then(function (response) {
      var res = response.data;

      $this.tmRandomConfig = res.items;

    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnGetConfigClick: function () {
    if (this.form.tmRandomType !== 'RandomNone') {
      this.apiGetConfig();
    }
  },

  tmRandomTypeChange: function (value) {
    this.tmRandomTypeChange = null;
    this.form.txIds = null;
    this.form.tmGroupIds = null;
    this.form.tmGroupPercentages = {};
    this.totalPercentage = 0;
    this.tmGroupList = [];
    if (value == 'RandomNone') {
      this.tmGroupList = this.tmFixedGroupList;
    }
    else {
      this.tmGroupList = this.tmAllGroupList;
      this.btnGetConfigClick();
    }
  },

  tmGroupIdsChange: function () {
    // 初始化新选择的题目组的占比
    if (!this.form.tmGroupPercentages) {
      this.form.tmGroupPercentages = {};
    }

    // 确保每个选中的题目组都有占比值
    if (this.form.tmGroupIds && this.form.tmGroupIds.length > 0) {
      var totalGroups = this.form.tmGroupIds.length;
      var defaultPercentage = Math.floor(100 / totalGroups);

      // 为新选择的题目组设置默认占比
      this.form.tmGroupIds.forEach(groupId => {
        if (!this.form.tmGroupPercentages[groupId]) {
          this.$set(this.form.tmGroupPercentages, groupId, defaultPercentage);
        }
      });

      // 移除未选中的题目组的占比
      for (var groupId in this.form.tmGroupPercentages) {
        if (!this.form.tmGroupIds.includes(parseInt(groupId))) {
          this.$delete(this.form.tmGroupPercentages, groupId);
        }
      }

      // 计算总占比
      this.calculateTotalPercentage();
    }

    // 调用原有的配置加载函数
    this.btnGetConfigClick();
  },

  calculateTotalPercentage: function () {
    var total = 0;
    if (this.form.tmGroupPercentages) {
      for (var groupId in this.form.tmGroupPercentages) {
        total += this.form.tmGroupPercentages[groupId] || 0;
      }
    }
    this.totalPercentage = total;
    return total;
  },

  getTmGroupName: function (groupId) {
    if (this.tmGroupList) {
      var group = this.tmGroupList.find(g => g.id === groupId);
      return group ? group.groupName : '未知题目组';
    }
    return '未知题目组';
  },

  balancePercentages: function () {
    if (this.form.tmGroupIds && this.form.tmGroupIds.length > 0) {
      var totalGroups = this.form.tmGroupIds.length;
      var equalPercentage = Math.floor(100 / totalGroups);
      var remainder = 100 - (equalPercentage * totalGroups);

      // 为每个题目组设置相等的占比
      this.form.tmGroupIds.forEach((groupId, index) => {
        // 将余数分配给前几个题目组
        var percentage = equalPercentage;
        if (index < remainder) {
          percentage += 1;
        }
        this.$set(this.form.tmGroupPercentages, groupId, percentage);
      });

      // 更新总占比显示
      this.calculateTotalPercentage();
    }
  },
  btnOpenEditClick: function (ref, ptype) {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getCommonUrl('editorOpenLayer', { pf: window.name, ptype: ptype, ref: ref }),
      width: "58%",
      height: "78%"
    });
  },
  btnSaveClick: function () {
    this.submitSubmitType = 'Save';
    this.submitSubmitIsClear = false;
    var $this = this;
    this.$refs.form.validate(function (valid) {
      if (valid) {
        var valido = $this.submitValid();
        if (!valido) return;

        $this.apiSubmit();
      }
    });

  },
  btnSubmitClick: function () {
    this.submitSubmitType = 'Submit';
    this.submitSubmitIsClear = false;

    if ((this.form.userGroupIds && this.form.userGroupIds.length > 0) || this.form.isCourseUse) {

      if (this.form.isCourseUse) {
        this.form.userGroupIds = null;
      }

      var $this = this;
      this.$refs.form.validate(function (valid) {
        if (valid) {
          var valido = $this.submitValid();
          if (!valido) return;

          if ($this.id > 0 && $this.form.submitType === 'Submit') {
            $this.submitDialogVisible = true;
            $this.submitSubmitIsClear = true;
          }
          else {
            $this.apiSubmit();
          }
        }
      });
    }
    else {
      utils.error('请选择至少一个用户组', { layer: true });
    }

  },
  btnSubmit: function () {
    this.submitSubmitIsClear = false;
    this.apiSubmit();
  },
  btnSubmitClear: function () {
    var $this = this;
    top.utils.alertWarning({
      title: '重新发布提醒',
      text: '确定清空后发布吗？',
      confirmButtonText: '重新发布',
      showCancelButton: true,
      callback: function () {
        $this.submitSubmitIsClear = true;
        $this.apiSubmit();
      }
    });

  },
  apiSubmit: function () {

    var $this = this;
    utils.loading($this, true);
    $api.post($url, {
      isClear: this.submitSubmitIsClear,
      submitType: this.submitSubmitType,
      item: $this.form,
      configList: $this.tmRandomConfig,
      isUpdateDateTime: $this.isUpdateDateTime,
      isUpdateExamTimes: $this.isUpdateExamTimes
    }).then(function (response) {
      var res = response.data;
      if (res.value) {
        utils.success("操作成功");
      }

    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
      utils.closeLayerSelf();
    });
  },
  submitValid: function () {
    if (this.form.examBeginDateTime >= this.form.examEndDateTime) {
      utils.error('请选择有效的考试时间', { layer: true });
      return false;
    }
    if (this.form.tmRandomType === 'RandomNone') {
      if (this.form.tmGroupIds === null || this.form.tmGroupIds.length === 0) {
        utils.error('请选择至少一个题目组', { layer: true });
        return false;
      }
    }

    // 验证题目组占比
    if (this.form.tmGroupIds && this.form.tmGroupIds.length > 0) {
      // 检查是否所有选中的题目组都有占比设置
      var allHavePercentage = true;
      this.form.tmGroupIds.forEach(groupId => {
        if (!this.form.tmGroupPercentages || this.form.tmGroupPercentages[groupId] === undefined) {
          allHavePercentage = false;
        }
      });

      if (!allHavePercentage) {
        utils.error('请为所有选中的题目组设置占比', { layer: true });
        return false;
      }

      // 检查总占比是否为100%
      var total = this.calculateTotalPercentage();
      if (total !== 100) {
        utils.error('题目组占比总和必须为100%，当前为' + total + '%', { layer: true });
        return false;
      }
    }

    return true;
  },
  getSummaries(param) {
    if (this.tmRandomConfig && this.tmRandomConfig.length > 0) {
      const { columns, data } = param;
      const sums = [];
      columns.forEach((column, index) => {
        if (index === 0) {
          sums[index] = '合计';
          return;
        }
        if (index === 3) {
          sums[index] = '';
          return;
        }
        if (index === 1) {
          let totalCount = 0;
          data.forEach(item => {
            totalCount += (item.nandu1TmCount + item.nandu2TmCount + item.nandu3TmCount + item.nandu4TmCount + item.nandu5TmCount);
          })

          sums[index] = '共 ' + totalCount + ' 道题';
        }
        if (index === 2) {

          let totalScore = 0;
          data.forEach(item => {
            totalScore += (item.nandu1TmCount + item.nandu2TmCount + item.nandu3TmCount + item.nandu4TmCount + item.nandu5TmCount) * item.txScore;
          })

          if (this.tmConfigDialogVisible) {
            sums[6] = '题型总分 ' + totalScore + ' 分';
          }
          else {
            sums[index] = '题型总分 ' + totalScore + ' 分';
          }


          this.txTotalScore = totalScore;

          if (this.form.tmScoreType === 'ScoreTypeTx') {
            this.form.totalScore = this.txTotalScore;
          }
        }

      });
      return sums;
    }

  },
  scoreTypeChange: function (value) {
    if (value === 'ScoreTypeTx' && this.form.tmRandomType !== 'RandomNone') {
      this.form.totalScore = this.txTotalScore;
    }
    else {
      this.form.totalScore = 100;
    }
  },
  frequencyTypeChange: function (value) {

  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});

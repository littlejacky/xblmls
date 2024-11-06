var $url = '/common/selectAuthOrgan';
var $urlSet = $url + '/set';

var data = utils.init({
  organs: null,
  search: '',
  expandRowKeys: [],
  defaultExpandAll: false,
});

var methods = {
  apiGet: function (message) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, { params: { search: this.search } }).then(function (response) {
      var res = response.data;
      $this.organs = res.organs;

    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnSearch: function () {
    this.apiGet();
  },
  handleSelectionChange(val) {
    this.multipleSelection = val;
  },
  btnSubmitClick: function (organ) {
    var $this = this;

    top.utils.alertWarning({
      title: '切换道管理组织提醒',
      text: '切换管理组织到【' + organ.name + '】,确定切换吗？',
      callback: function () {
        $this.apiSet(organ.id);
      }
    });
  },
  apiSet: function (id) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlSet, { id: id }).then(function (response) {
      var res = response.data;
      if (res.value) {
        top.utils.alertSuccess({
          title: '切换成功',
          button: '点击跳转',
          callback: function () {
            top.location.href = utils.getIndexUrl();
          }
        });
      }
      else {
        utils.error("切换失败，请重试", { layer: true });
      }

    }).catch(function (error) {
      utils.error(error, { layer: true });
    }).then(function () {
      utils.loading($this, false);
    });
  },
  rowClick: function (row, column, event) {
    this.$refs.organsTable.toggleRowExpansion(row);
  },
  //  树形表格过滤
  handleTreeData(treeData, searchValue) {
    if (!treeData || treeData.length === 0) {
      return [];
    }
    const array = [];
    for (let i = 0; i < treeData.length; i += 1) {
      let match = false;
      for (let pro in treeData[i]) {
        if (typeof (treeData[i][pro]) == 'string') {
          match |= treeData[i].name.includes(searchValue);
          if (match) break;
        }
      }
      if (this.handleTreeData(treeData[i].children, searchValue).length > 0 || match) {
        array.push({
          ...treeData[i],
          children: this.handleTreeData(treeData[i].children, searchValue),
        });
      }
    }
    return array;
  },
  // 将过滤好的树形数据展开
  setExpandRow(handleTreeData) {
    if (handleTreeData.length) {
      for (let i of handleTreeData) {
        this.expandRowKeys.push(i.guid)
        if (i.children.length) {
          this.setExpandRow(i.children)
        }
      }
    }
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  computed: {
    treeTable: function () {
      var searchValue = this.search;
      if (searchValue) {
        let treeData = this.organs;
        let handleTreeData = this.handleTreeData(treeData, searchValue);
        this.defaultExpandAll = true;
        this.setExpandRow(handleTreeData);
        return handleTreeData;
      }
      this.expandRowKeys.push(this.organs[0].guid);
      return this.organs;
    }
  },
  created: function () {
    this.apiGet();

  }
});

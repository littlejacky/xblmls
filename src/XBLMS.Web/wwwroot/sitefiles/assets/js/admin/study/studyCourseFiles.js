var $url = 'study/studyCourseFiles';

var $urlActionsDeleteFile = $url + '/file/del';
var $urlActionsDeleteGroup = $url + '/group/del';
var $urlActionsDeleteGroupAndFile = $url + '/delList';

var $urlActionsDownload = $url + '/file/download';

var data = utils.init({
  form: {
    keyword: '',
    groupId: 0,
  },
  list: null,
  groupId: 0,
  paths: null,
  curMouseoverId: 0,
  curFileType: ''
});

var methods = {
  apiList: function () {
    var $this = this;
    utils.loading(this, true);
    $api.get($url, {
      params: this.form
    }).then(function (response) {
      var res = response.data;
      $this.paths = res.paths;
      $this.list = res.list;
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },
  btnGroupAddClick: function (id) {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getStudyUrl('studyCourseFilesGroupEdit', { id: id, groupId: this.groupId }),
      width: "38%",
      height: "38%",
      end: function () {
        $this.apiList();
      }
    });
  },
  btnEditClick: function (row) {
    this.btnGroupAddClick(row.id);
  },
  btnDeleteClick: function (row) {
    var $this = this;
    if (row.type === 'Group') {
      top.utils.alertDelete({
        title: '删除文件夹',
        text: '确定删除文件夹吗',
        callback: function () {
          $this.apiDeleteGroup(row.id);
        }
      });
    }
    else {
      top.utils.alertDelete({
        title: '删除文件',
        text: '确定删除此文件吗？',
        callback: function () {
          $this.apiDeleteFile(row.id);
        }
      });
    }
  },
  apiDeleteGroup: function (id) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlActionsDeleteGroup, {
      id: id
    }).then(function (response) {
      var res = response.data;

    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
      $this.apiList();
    });
  },
  btnDeleteMoreClick: function () {
    var $this = this;

    var nodes = this.$refs.fileTable.selection;
    var ids = _.map(nodes, function (item) {
      return item.id;
    });
    var files = [];
    for (var i = 0; i < nodes.length; i++) {
      files.push({ id: nodes[i].id, type: nodes[i].type })
    }
    if (files.length > 0) {
      top.utils.alertDelete({
        title: '批量删除文件',
        text: '此操作将删除选中的文件，确认删除吗？',
        callback: function () {
          $this.apiDeleteFileAndGroup(files);
        }
      });
    }
    else {
      utils.error("请选择要删除的内容");
    }

  },
  apiDeleteFileAndGroup: function (files) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlActionsDeleteGroupAndFile, {
      files: files
    }).then(function (response) {
      var res = response.data;
      utils.success("操作成功");
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
      $this.apiList();
    });
  },
  apiDeleteFile: function (id) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlActionsDeleteFile, {
      id: id
    }).then(function (response) {
      var res = response.data;
      utils.success("操作成功");
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
      $this.apiList();
    });
  },
  btnTitleClick: function (material) {
    this.groupId = this.form.groupId = material.id;
    this.apiList();
  },
  breadcrumbPath: function (id) {
    this.groupId = this.form.groupId = id;
    this.apiList();
  },
  btnUploadMore: function () {
    var $this = this;
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: utils.getStudyUrl('studyCourseFilesUpload', { groupId: this.groupId }),
      width: "88%",
      height: "98%",
      end: function () {
        $this.apiList();
      }
    });
  },

  btnDownloadClick: function (id) {
    window.open($apiUrl + $urlActionsDownload + '?id=' + id + '&access_token=' + $token);
  },
  btnView: function (id) {
    var url = utils.getCommonUrl('studyCourseFileLayerView', { id: id });
    top.utils.openLayer({
      title: false,
      closebtn: 0,
      url: url,
      width: "68%",
      height: "98%"
    });
  },

  mouseoverShowIn: function (row, column, cell, event) {
    this.curMouseoverId = row.id;
    this.curFileType = row.type;
  },
  mouseoverShowOut: function (row, column, cell, event) {
    this.curMouseoverId = 0;
    this.curFileType = '';
  },
  fileTableSelectable: function (row, index) {
    return true;
  },
  btnSearchClick: function () {
    this.apiList();
  },
  fileTableBySelectable: function (row, index) {
    return row.type==='File';
  },
  btnSelectClick: function () {
    var $this = this;

    var nodes = this.$refs.fileTable.selection;
    var ids = _.map(nodes, function (item) {
      return item.id;
    });
    var files = [];
    for (var i = 0; i < nodes.length; i++) {
      files.push(nodes[i])
    }
    if (files.length > 0) {
      var parentFrameName = utils.getQueryString("pf");
      var parentLayer = top.frames[parentFrameName];
      parentLayer.$vue.selectFilesCallback(files);
      utils.closeLayerSelf();
    }
    else {
      utils.error("请至少选中一个课件", { layer: true });
    }
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiList();
  }
});

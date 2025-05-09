var $url = "/analysis";

var data = utils.init({
  guestToken: "",
});

var methods = {
  apiGet: function(callback) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url).then(function (response) {
      var res = response.data;
      $this.guestToken = res.guestToken;
      if ($this.guestToken)
        callback && callback();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },
  test: function () {
    var $this = this;

  }

};

Vue.component("superset-ui", {
  template: `
    <div id="my-superset-container" style="position:absolute; inset:0;"></div>
  `,
  mounted: function () {
    const dashboard = supersetEmbeddedSdk.embedDashboard({
      id: "fdb0e4e3-ac5b-438e-b484-ffe763975c5e", // given by the Superset embedding UI
      supersetDomain: "https://bi.maxitai.cn",
      mountPoint: document.getElementById("my-superset-container"), // any html element that can contain an iframe
      fetchGuestToken: () => this.$root.guestToken,
      dashboardUiConfig: { // dashboard UI config: hideTitle, hideTab, hideChartControls, filters.visible, filters.expanded (optional), urlParams (optional)
        hideTitle: true,
        filters: {
          expanded: true,
        },
        urlParams: {
          //foo: 'value1',
          //bar: 'value2',
          // ...
        }
      },
      // optional additional iframe sandbox attributes
      iframeSandboxExtras: ['allow-top-navigation', 'allow-popups-to-escape-sandbox']
    }).catch(() => {
      location.reload();
    });

    const iframe = document.querySelector('#my-superset-container iframe');
    iframe.width = '100%';
    iframe.height = '100%';
    iframe.style.border = 'none';
  }
});

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  },
});

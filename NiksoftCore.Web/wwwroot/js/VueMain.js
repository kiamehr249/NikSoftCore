


$(document).ready(function () {
	getInstance({
		el: '#wbasket',
		mixins: [utilities],
		data: {
			bUrl: '/api/PurchaseApi/',
			bUrl2: '/api/base/AccountApi/',
			basket: {
				items: []
            }
		},
		created: function () {
			this.GetBasket();
		},
		methods: {
			GetBasket: function () {
				let request = {
				};
				var self = this;
				this.loadJson(this.bUrl + "GetBasketItems", request)
					.then((apiResults) => {
						if (apiResults.status === 200) {
							//self.showMessage({ message: 'ارسال با موفقیت انجام شد', type: 'success' });
							self.basket.items = apiResults.data;
						} else {
							self.showMessage({ message: apiResults.message, type: 'error' });
						}
					});
			},
			SetBasket: function (productId, countOf, succFunc) {
				let request = {
					productId: productId,
					countOf: countOf
				};
				var self = this;
				this.loadJson(this.bUrl + "SetBasketItem", request)
					.then((apiResults) => {
						if (apiResults.status === 200) {
							self.showMessage({ message: 'محصول به سبد خرید اضافه شد', type: 'success' });
							self.basket.items.push(apiResults.data);
							succFunc();
						} else {
							self.showMessage({ message: apiResults.message, type: 'error' });
						}
					});
			},
			RemoveBasket: function () {
				let request = {
				};
				var self = this;
				this.loadJson(this.bUrl + "RemoveBasket", request)
					.then((apiResults) => {
						if (apiResults.status === 200) {
							self.showMessage({ message: 'سبد خرید شما خالی شد.', type: 'success' });
							self.basket.items = [];
						} else {
							self.showMessage({ message: apiResults.message, type: 'error' });
						}
					});
			}
		}
	}, 0);


	if ($('#basketmanage').length > 0) {
        getInstance({
            el: '#basketmanage',
            mixins: [utilities],
            data: {
                bUrl: '/api/PurchaseApi/',
                bUrl2: '/api/base/AccountApi/',
                basket: {
                    items: []
                }
            },
            created: function () {
                this.GetBasket();
            },
            methods: {
				GetBasket: function () {
					let request = {
					};
					var self = this;
					this.loadJson(this.bUrl + "GetBasketAll", request)
						.then((apiResults) => {
							if (apiResults.status === 200) {
								//self.showMessage({ message: 'ارسال با موفقیت انجام شد', type: 'success' });
								self.basket.items = apiResults.data;
							} else {
								self.showMessage({ message: apiResults.message, type: 'error' });
							}
						});
				},
				RemoveBasketItem: function (itemId) {
					var request = {
						id: itemId
                    };
                    var self = this;
					this.loadJson(this.bUrl + "RemoveBasketItem", request)
                        .then((apiResults) => {
                            if (apiResults.status === 200) {
								self.showMessage({ message: 'حذف انجام شد.', type: 'success' });
								var indx = self.basket.items.findIndex(x => x.id == apiResults.data);
								self.basket.items.splice(indx, 1);
                            } else {
                                self.showMessage({ message: apiResults.message, type: 'error' });
                            }
                        });
                }
            }
        }, 1);
    }





});
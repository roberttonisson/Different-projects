import { bootstrap } from 'aurelia-bootstrapper';
import { DefaultToppingService } from './../../service/default-topping-service';
import { AdditionalToppingService } from './../../service/additional-topping-service';
import { RouteConfig, NavigationInstruction, Router } from 'aurelia-router';
import { CartService } from './../../service/cart-service';
import { PizzaInCartService } from './../../service/pizza-in-cart-service';
import { IPizzaInCart } from './../../domain/IPizzaInCart';
import { ToppingService } from './../../service/topping-service';
import { SizeService } from './../../service/size-service';
import { ITopping } from './../../domain/ITopping';
import { ISize } from './../../domain/ISize';
import { autoinject } from 'aurelia-framework';
import { AlertType } from "types/AlertType";
import { PizzaTypeService } from "service/pizza-type-service";
import { IAlertData } from "types/IAlertData";
import { IPizzaType } from "domain/IPizzaType";
import { ICrust } from 'domain/ICrust';
import { CrustService } from 'service/crust-service';
import { ICart } from 'domain/ICart';
import 'style/pizzasDrinks.css'
import { IDefaultTopping } from 'domain/IDefaultTopping';
import { AppState } from 'state/app-state';
import { PizzaResources } from './../../lang/pizzas';


@autoinject
export class PizzasIndex {

    private langResources = PizzaResources;

    private _pizzas: IPizzaType[] = [];

    private _crusts: ICrust[] = [];

    private _sizes: ISize[] = [];

    private _toppings: ITopping[] = [];

    private _defaultToppings: IDefaultTopping[] = [];


    private _alert: IAlertData | null = null;


    private _cart: ICart | null = null;

    private _pizza: IPizzaType | null = null;

    private _crust: ICrust | null = null;

    private _size: ICrust | null = null;

    private _pizzaInCart: IPizzaInCart | null = null;

    private _additional: ITopping[] = [];

    private _default: ITopping[] = [];

    private _available: ITopping[] = [];

    private _quantity: number = 1;
    private _row: number = 0;




    constructor(
        private appState: AppState,
        private pizzaService: PizzaTypeService,
        private crustService: CrustService,
        private sizeService: SizeService,
        private toppingService: ToppingService,
        private pizzaInCartService: PizzaInCartService,
        private cartService: CartService,
        private additionalToppingService: AdditionalToppingService,
        private defaultToppingService: DefaultToppingService,
        private router: Router
    ) {

    }
    

    attached() {


        this.cartService.getActive().then(
            response => {
                if (response.statusCode >= 200 && response.statusCode < 300) {
                    this._alert = null;
                    this._cart = response.data!;
                } else {
                    // show error message
                    this._alert = {
                        message: response.statusCode.toString() + ' - ' + response.errorMessage,
                        type: AlertType.Danger,
                        dismissable: true,
                    }
                }
            }
        );

        this.defaultToppingService.getAll().then(
            response => {
                if (response.statusCode >= 200 && response.statusCode < 300) {
                    this._alert = null;
                    this._defaultToppings = response.data!;
                } else {
                    // show error message
                    this._alert = {
                        message: response.statusCode.toString() + ' - ' + response.errorMessage,
                        type: AlertType.Danger,
                        dismissable: true,
                    }
                }
            }
        );

        this.pizzaService.getAll().then(
            response => {
                if (response.statusCode >= 200 && response.statusCode < 300) {
                    this._alert = null;
                    this._pizzas = response.data!;
                    this._row = Math.ceil(this._pizzas.length / 3)
                } else {
                    // show error message
                    this._alert = {
                        message: response.statusCode.toString() + ' - ' + response.errorMessage,
                        type: AlertType.Danger,
                        dismissable: true,
                    }
                }
            }
        );

        this.crustService.getAll().then(
            response => {
                if (response.statusCode >= 200 && response.statusCode < 300) {
                    this._alert = null;
                    this._crusts = response.data!;
                    this._crust = this._crusts[0];
                } else {
                    // show error message
                    this._alert = {
                        message: response.statusCode.toString() + ' - ' + response.errorMessage,
                        type: AlertType.Danger,
                        dismissable: true,
                    }
                }
            }
        );

        this.toppingService.getAll().then(
            response => {
                if (response.statusCode >= 200 && response.statusCode < 300) {
                    this._alert = null;
                    this._toppings = response.data!;
                } else {
                    // show error message
                    this._alert = {
                        message: response.statusCode.toString() + ' - ' + response.errorMessage,
                        type: AlertType.Danger,
                        dismissable: true,
                    }
                }
            }
        );

        this.sizeService.getAll().then(
            response => {
                if (response.statusCode >= 200 && response.statusCode < 300) {
                    this._alert = null;
                    this._sizes = response.data!;
                    this._size = this._sizes[0];
                } else {
                    // show error message
                    this._alert = {
                        message: response.statusCode.toString() + ' - ' + response.errorMessage,
                        type: AlertType.Danger,
                        dismissable: true,
                    }
                }
            }
        );

    }

    activate(params: any, routeConfig: RouteConfig, navigationInstruction: NavigationInstruction) {

    }

    selectPizza(pizza: IPizzaType) {
        if (this.appState.jwt == null) {
            this.router.navigateToRoute('account-login');
            return;
        }
        this._pizza = null;
        this._additional = [];
        this._available = [];
        this._default = []
        this._pizza = pizza;
        for (const defaultTopping of this._defaultToppings) {
            if (defaultTopping.pizzaTypeId === pizza.id) {
                this._default.push(defaultTopping.topping!)
            }
        }
        this.availableToppings();
    }

    availableToppings() {
        for (const t of this._toppings) {
            let inDefault = false;
            for (const d of this._default) {
                if (d.id == t.id) {
                    inDefault = true;
                }
            }
            if (!inDefault) {
                this._available.push(t)
            }

        }

    }

    chooseCrust(crust: ICrust) {
        this._crust = crust;
    }

    chooseSize(size: ISize) {
        this._size = size;
    }

    plus() {
        this._quantity = Number(this._quantity) + 1;
    }
    minus() {
        this._quantity = Number(this._quantity) - 1;
        if (this._quantity < 1) {
            this._quantity = 1;
        }
    }

    addToCart(event: Event) {
        this.pizzaInCartService
            .create({
                pizzaTypeId: this._pizza!.id,
                cartId: this._cart!.id,
                crustId: this._crust!.id,
                sizeId: this._size!.id,
                quantity: this._quantity,
                id: undefined
            })
            .then(
                response => {
                    if (response.statusCode >= 200 && response.statusCode < 300) {
                        this._alert = null;
                        this._pizzaInCart = response.data!
                        for (const topping of this._additional) {
                            this.additionalToppingService
                                .create({
                                    pizzaInCartId: this._pizzaInCart!.id,
                                    toppingId: topping.id,
                                    id: undefined
                                })
                                .then(
                                    response => {
                                        if (response.statusCode >= 200 && response.statusCode < 300) {
                                            this._alert = null;
                                        } else {
                                            // show error message
                                            this._alert = {
                                                message: response.statusCode.toString() + ' - ' + response.errorMessage,
                                                type: AlertType.Danger,
                                                dismissable: true,
                                            }
                                        }
                                    }
                                );
                        }
                    } else {
                        // show error message
                        this._alert = {
                            message: response.statusCode.toString() + ' - ' + response.errorMessage,
                            type: AlertType.Danger,
                            dismissable: true,
                        }
                    }
                }
            );
        event.preventDefault();
    }

    get total(): number {
        let total = 0;
        if (this._pizza != null) {
            total = total + this._pizza.price
        }
        if (this._crust != null) {
            total = total + this._crust.price
        }
        if (this._size != null) {
            total = total + this._size.price
        }
        for (const topping of this._additional) {
            total = total + topping.price
        }

        

        return total * this._quantity;
    }

}

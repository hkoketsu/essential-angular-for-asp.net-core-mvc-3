import { Component } from '@angular/core';
import { Repository } from './models/repository';
import { Product } from './models/product.model';
import { Supplier } from './models/supplier.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'SportsStore';

  constructor(private repo: Repository) { }

  get product(): Product {
    return this.repo.product;
  }

  get products(): Product[] {
    return this.repo.products;
  }

  createProduct() {
    this.repo.createProduct(new Product(0, "X-Ray Scuba Mask", "Watersports",
      "See what the fish are hiding", 49.99, this.repo.products[0].supplier));
  }

  createProductAndSupplier() {
    let s = new Supplier(0, "Rocket Shoe Corp", "Boston", "MA");
    let p = new Product(0, "Rocket-Powered Shoes", "Running", "Set a new record", 100, s);
    this.repo.createProductAndSupplier(p, s);
  }

  replaceProduct() {
    const p = this.repo.products[0];
    p.name = "Modified Product";
    p.category = "Modified Category";
    this.repo.replaceProduct(p);
  }

  replaceSupplier() {
    const s = new Supplier(3, "Modified Supplier", "New York", "NY");
    this.repo.replaceSupplier(s);
  }

  deleteProduct() {
    this.repo.deleteProduct(1);
  }

  deleteSupplier() {
    this.repo.deleteSupplier(2);
  }
}

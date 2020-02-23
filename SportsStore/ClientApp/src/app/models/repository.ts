import { Product } from './product.model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Filter } from "./configClasses.repository"
import { Supplier } from './supplier.model';

const productsUrl = "/api/products";
const suppliersUrl = "/api/suppliers";

@Injectable()
export class Repository {
  product: Product;
  products: Product[];
  suppliers: Supplier[];
  filter: Filter = new Filter();

  constructor(private http: HttpClient) {
    this.filter.related = true;
    this.getProducts();
  }

  getProduct(id: number) {
    this.http.get<Product>(`${productsUrl}/${id}`)
      .subscribe(p => this.product = p);
  }

  getProducts(related = false) {
    let url = `${productsUrl}?related=${this.filter.related}`;
    if (this.filter.category) {
      url += `&category=${this.filter.category}`;
    }
    if (this.filter.search) {
      url += `&search=${this.filter.search}`;
    }
    this.http.get<Product[]>(url).subscribe(products => this.products = products);
  }

  createProduct(product: Product) {
    const data = {
      name: product.name,
      category: product.category,
      description: product.description,
      price: product.price,
      supplier: product.supplier ? product.supplier.supplierId : 0
    };

    this.http.post<number>(productsUrl, data)
      .subscribe(id => {
        product.productId = id;
        this.products.push(product);
      })
  }

  createProductAndSupplier(product: Product, supplier: Supplier) {
    const data = {
      name: supplier.name,
      city: supplier.city,
      state: supplier.state
    };

    this.http.post<number>(suppliersUrl, data)
      .subscribe(id => {
        supplier.supplierId = id;
        product.supplier = supplier;
        this.suppliers.push(supplier);
        if (product != null) {
          this.createProduct(product);
        }
      });
  }
}

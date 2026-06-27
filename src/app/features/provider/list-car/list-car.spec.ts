import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListCar } from './list-car';

describe('ListCar', () => {
  let component: ListCar;
  let fixture: ComponentFixture<ListCar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListCar],
    }).compileComponents();

    fixture = TestBed.createComponent(ListCar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

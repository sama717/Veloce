import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScrollingHero } from './scrolling-hero';

describe('ScrollingHero', () => {
  let component: ScrollingHero;
  let fixture: ComponentFixture<ScrollingHero>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScrollingHero],
    }).compileComponents();

    fixture = TestBed.createComponent(ScrollingHero);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

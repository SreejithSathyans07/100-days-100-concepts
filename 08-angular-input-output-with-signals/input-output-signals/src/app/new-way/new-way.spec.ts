import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewWay } from './new-way';

describe('NewWay', () => {
  let component: NewWay;
  let fixture: ComponentFixture<NewWay>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewWay]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewWay);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

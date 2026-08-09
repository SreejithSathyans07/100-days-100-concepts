import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EffectSignals } from './effect-signals';

describe('EffectSignals', () => {
  let component: EffectSignals;
  let fixture: ComponentFixture<EffectSignals>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EffectSignals]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EffectSignals);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

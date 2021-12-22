import {Control} from 'ol/control';

export class RotateRightControl extends Control {
	constructor() {
		super({});

    const button = document.createElement('div');
    button.innerHTML = '<button title="Rotate right"><i class="bi bi-arrow-90deg-right"></i></button>'

		const element = document.createElement('div');
		element.className = 'ol-feature ol-control';
		element.style.marginLeft = '37px';
    element.style.marginTop = '137px';
		element.appendChild(button);

		Control.call(this, {
			element: element
		});

		button.addEventListener('click', this.rotateRight.bind(this), false);
	}

	rotateRight(): void {
		const rotation = super.getMap().getView().getRotation();
		super.getMap().getView().setRotation(rotation + 0.2);
	}
}

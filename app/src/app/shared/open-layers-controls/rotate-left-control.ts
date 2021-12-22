import {Control} from 'ol/control';

export class RotateLeftControl extends Control {
	constructor() {
		super({});

		const button = document.createElement('div');
		button.innerHTML = '<button title="Rotate left"><i class="bi bi-arrow-90deg-left"></i></button>'

		const element = document.createElement('div');
		element.className = 'ol-feature ol-control';
		element.style.marginLeft = '8px';
		element.style.marginTop = '137px';
		element.appendChild(button);

		Control.call(this, {
			element: element
		});

		button.addEventListener('click', this.rotateLeft.bind(this), false);
	}

	rotateLeft(): void {
		const rotation = super.getMap().getView().getRotation();
		super.getMap().getView().setRotation(rotation - 0.2);
	}
}

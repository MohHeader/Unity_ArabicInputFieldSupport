using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArabicInputField : InputField {
	private string o_text = "";

	protected override void Append (char input)
	{
		if (!this.InPlaceEditing ())
		{
			return;
		}
		if (this.onValidateInput != null)
		{
			input = this.onValidateInput (this.text, this.caretPositionInternal, input);
		}
		else
		{
			if (this.characterValidation != InputField.CharacterValidation.None)
			{
				input = this.Validate (this.text, this.caretPositionInternal, input);
			}
		}
		if (input == '\0')
		{
			return;
		}
		this.Insert (input);
	}

	private bool InPlaceEditing ()
	{
		return !TouchScreenKeyboard.isSupported;
	}

	private void Insert (char c)
	{
		string text = c.ToString ();
		o_text += "";

		this.Delete ();
		if (this.characterLimit > 0 && this.text.Length >= this.characterLimit)
		{
			return;
		}
		o_text = o_text.Insert (this.m_CaretPosition, text);
		this.m_Text = ArabicSupport.ArabicFixer.Fix(o_text, false, true);
		this.caretSelectPositionInternal = (this.caretPositionInternal += text.Length );
	}

	private void Delete ()
	{
		if (this.caretPositionInternal == this.caretSelectPositionInternal)
		{
			return;
		}
		if (this.caretPositionInternal < this.caretSelectPositionInternal)
		{
			this.m_Text = this.text.Substring (0, this.caretPositionInternal) + this.text.Substring (this.caretSelectPositionInternal, this.text.Length - this.caretSelectPositionInternal);
			this.caretSelectPositionInternal = this.caretPositionInternal;
		}
		else
		{
			this.m_Text = this.text.Substring (0, this.caretSelectPositionInternal) + this.text.Substring (this.caretPositionInternal, this.text.Length - this.caretPositionInternal);
			this.caretPositionInternal = this.caretSelectPositionInternal;
		}
	}


	protected void KeyPressed2 (Event evt){
		switch (evt.keyCode) {
		case KeyCode.Backspace:
			if (this.caretPositionInternal > 0)
				this.o_text = this.o_text.Remove (this.caretPositionInternal - 1, 1);
			break;
		}

	}

	private Event m_ProcessingEvent = new Event ();
	public override void OnUpdateSelected (UnityEngine.EventSystems.BaseEventData eventData)
	{
		if (!this.isFocused)
		{
			return;
		}
		bool flag = false;
		while (Event.PopEvent (this.m_ProcessingEvent))
		{
			if (this.m_ProcessingEvent.rawType == EventType.KeyDown)
			{
				flag = true;
				this.KeyPressed2 (this.m_ProcessingEvent);
				InputField.EditState editState = this.KeyPressed (this.m_ProcessingEvent);
				if (editState == InputField.EditState.Finish)
				{
					this.DeactivateInputField ();
					break;
				}
			}
		}
		if (flag)
		{
			this.UpdateLabel ();
		}
		eventData.Use ();
	}


}

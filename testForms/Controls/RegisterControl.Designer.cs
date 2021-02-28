
namespace testForms.Controls
{
    partial class RegisterControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.username_tb = new System.Windows.Forms.TextBox();
            this.email_tb = new System.Windows.Forms.TextBox();
            this.password_tb = new System.Windows.Forms.TextBox();
            this.register_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(40, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Регистрация";
            // 
            // username_tb
            // 
            this.username_tb.Location = new System.Drawing.Point(16, 54);
            this.username_tb.Name = "username_tb";
            this.username_tb.PlaceholderText = "Username";
            this.username_tb.Size = new System.Drawing.Size(171, 23);
            this.username_tb.TabIndex = 1;
            // 
            // email_tb
            // 
            this.email_tb.Location = new System.Drawing.Point(16, 92);
            this.email_tb.Name = "email_tb";
            this.email_tb.PlaceholderText = "Email";
            this.email_tb.Size = new System.Drawing.Size(171, 23);
            this.email_tb.TabIndex = 2;
            // 
            // password_tb
            // 
            this.password_tb.Location = new System.Drawing.Point(16, 132);
            this.password_tb.Name = "password_tb";
            this.password_tb.PasswordChar = '*';
            this.password_tb.PlaceholderText = "Password";
            this.password_tb.Size = new System.Drawing.Size(171, 23);
            this.password_tb.TabIndex = 3;
            // 
            // register_btn
            // 
            this.register_btn.Location = new System.Drawing.Point(16, 192);
            this.register_btn.Name = "register_btn";
            this.register_btn.Size = new System.Drawing.Size(171, 37);
            this.register_btn.TabIndex = 4;
            this.register_btn.Text = "Зарегистрироватся";
            this.register_btn.UseVisualStyleBackColor = true;
            this.register_btn.Click += new System.EventHandler(this.register_btn_Click);
            // 
            // RegisterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.register_btn);
            this.Controls.Add(this.password_tb);
            this.Controls.Add(this.email_tb);
            this.Controls.Add(this.username_tb);
            this.Controls.Add(this.label1);
            this.Location = new System.Drawing.Point(0, 55);
            this.Name = "RegisterControl";
            this.Size = new System.Drawing.Size(210, 254);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox username_tb;
        private System.Windows.Forms.TextBox email_tb;
        private System.Windows.Forms.TextBox password_tb;
        private System.Windows.Forms.Button register_btn;
    }
}

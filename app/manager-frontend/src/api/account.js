export const getAccountApi = ({ api }) => ({
  /**
   * Register a new User account
   * @param {*} body
   */
  register: (values) =>
    api.post("account/register", {
      json: values,
    }),

  /**
   * Login a user, given their username and password
   * @param {*} values
   */
  login: (values) =>
    api.post("account/login", {
      json: values,
    }),

  /**
   * Logout the current user
   */
  logout: () => api.post("account/logout"),

  /**
   * Try to confirm a User account
   * @param {*} userId The User ID
   * @param {*} token The previous Account Confirmation token
   * @returns
   */
  confirm: (userId, token) =>
    api.post("account/confirm", {
      json: { userId, token },
    }),

  /**
   * Request a User's password reset link
   * @param {*} id User ID for requesting password reset link
   * @returns
   */
  generatePasswordResetLink: ({ id }) =>
    api.post(`account/${id}/password/reset`),

  /**
   * Reset a User's password, using a valid token
   * @param {*} userId User ID to reset password for
   * @param {*} token System issued password reset token
   * @param {*} password the new password
   * @param {*} passwordConfirm confirm the new password
   * @returns
   */
  resetPassword: (userId, token, password, passwordConfirm) =>
    api.post("account/password", {
      json: {
        credentials: { userId, token },
        data: { password, passwordConfirm },
      },
    }),

  /**
   * Request a User's account activation link
   * @param {*} id User ID for requesting account activation link
   * @returns
   */
  generateActivationLink: ({ id }) => api.post(`account/${id}/activation`),

  /**
   * Activate Users's account, using a valid token
   * @param {*} userId User ID to activate account for
   * @param {*} token System issued account activation token
   * @param {*} password the password
   * @param {*} fullName User full name
   * @returns
   */
  activateAccount: (userId, token, password, fullName) =>
    api.post("account/activate", {
      json: {
        credentials: { userId, token },
        data: { password, fullName },
      },
    }),
});

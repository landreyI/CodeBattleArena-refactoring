export class StandardError extends Error {
    fieldErrors?: Record<string, string>;
    code?: string;

    constructor(message: string, options?: {
        fieldErrors?: Record<string, string>;
        code?: string;
    }) {
        super(message);
        this.name = "StandardError";
        this.fieldErrors = options?.fieldErrors;
        this.code = options?.code;

        if (Error.captureStackTrace) {
            Error.captureStackTrace(this, StandardError);
        }
    }
}

export function processError(err: unknown): StandardError {
    let fieldErrors: Record<string, string> = {};
    let errorMessage = "An unexpected error occurred. Please try again.";

    // Axios error
    if (
        err &&
        typeof err === "object" &&
        "response" in err &&
        err.response &&
        typeof err.response === "object"
    ) {
        const response = (err as any).response;

        if (response.data) {
            const data = response.data;

            if (typeof data.error === "string") {
                errorMessage = data.error;
            }

            const code = typeof data.code === "string" ? data.code : undefined;

            if (typeof data.details === "object" && data.details !== null) {
                for (const key in data.details) {
                    const value = data.details[key];
                    if (typeof value === "string") {
                        fieldErrors[key.toLowerCase()] = value;
                    }
                }

                if (!errorMessage && Object.values(fieldErrors).length > 0) {
                    errorMessage = Object.values(fieldErrors)[0];
                }
            }

            return new StandardError(errorMessage, { fieldErrors, code });
        }
    }

    // ASP.NET Core ModelState errors
    if (err && typeof err === "object" && "errors" in err) {
        const errors = (err as { errors: Record<string, string[]> }).errors;
        for (const key in errors) {
            if (Array.isArray(errors[key])) {
                fieldErrors[key.toLowerCase()] = errors[key].join(", ");
            }
        }

        errorMessage = fieldErrors["form"] || Object.values(fieldErrors)[0] || errorMessage;
        return new StandardError(errorMessage, { fieldErrors });
    }

    // JS Error
    if (err instanceof Error) {
        return new StandardError(err.message);
    }

    // Fallback
    return new StandardError(errorMessage);
}


import SessionForm from "@/components/forms/SessionForm";


export function CreateSession() {
  return (
      <div className="glow-box">
          <div className="md:w-[30vw] sm:w-full mx-auto">
              <div className="flex items-center justify-between mb-6">
                  <h1 className="text-4xl font-bold text-primary font-mono">
                      Create Session
                  </h1>
              </div>
              <SessionForm submitLabel="Created"></SessionForm>
          </div>
      </div>
  );
}

export default CreateSession;
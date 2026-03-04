import LeagueForm from "@/components/forms/LeagueForm";


export function CreateLeague() {
  return (
      <div className="glow-box">
          <div className="md:w-[30vw] sm:w-full mx-auto">
              <div className="flex items-center justify-between mb-6">
                  <h1 className="text-4xl font-bold text-green-400 font-mono">
                      Create League
                  </h1>
              </div>
              <LeagueForm submitLabel="Created"></LeagueForm>
          </div>
      </div>
  );
}

export default CreateLeague;
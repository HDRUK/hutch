import { useDisclosure, VStack, Text, useToast } from "@chakra-ui/react";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useUserList } from "api/user";
import { UserSummary } from "components/users/UserSummary";
import { ActionList } from "components/ActionList";
import { DeleteModal } from "components/DeleteModal";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { useUser } from "contexts/User";

export const UsersList = () => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedUser, setSelectedUser] = useState();
  const [isLoading, setIsLoading] = useState();
  const { users } = useBackendApi();
  const { user } = useUser();
  const { data, mutate } = useUserList(); // get users list data from the backend
  const toast = useToast();

  const { setFilter, outputList } = useSortingAndFiltering(data, "fullName", {
    initialSort: { key: "fullName" },
    sorters: {
      fullName: {
        sorter: (asc) => (a, b) =>
          asc ? a.localeCompare(b) : b.localeCompare(a),
      },
    },
    storageKey: "user",
  });

  const onDeleteUser = async () => {
    setIsLoading(true);
    await users.delete({ id: selectedUser.id });
    await mutate();
    onClose();
    setIsLoading(false);
    toast({
      position: "top",
      title: `User ${selectedUser.fullName} deleted!`,
      status: "success",
      duration: 1500,
      isClosable: true,
    });
  };

  const onClickDelete = (user) => {
    setSelectedUser(user);
    onOpen();
  };

  const ModalDelete = () => {
    return (
      <DeleteModal
        title={`Delete User?`}
        body={
          <VStack>
            <Text>Are you sure you want to delete this user:</Text>
            <Text fontWeight="bold">{selectedUser?.fullName}</Text>
          </VStack>
        }
        isOpen={isOpen}
        onClose={onClose}
        onDelete={onDeleteUser}
        isLoading={isLoading}
      />
    );
  };

  return (
    <ActionList
      data={outputList.length > 0}
      setFilter={setFilter}
      href="/users"
      actionTitle="User"
      actionNewTitle="Register an user"
      modalDelete={ModalDelete}
    >
      {outputList.length > 0 &&
        outputList.map((item, index) => (
          <>
            <UserSummary
              key={index}
              userId={item.id}
              href={`/users/${item.id}`}
              name={item.fullName}
              username={item.username}
              isUserActive={item.accountConfirmed}
              onDelete={
                // Prevent user from deleting itself & superadmin
                // exclude delete action from the summary card for the logged in user & admin
                !item.isProtected &&
                item.fullName !== user.fullName &&
                item.email !== user.email &&
                (() => onClickDelete(item))
              }
            />
          </>
        ))}
    </ActionList>
  );
};
